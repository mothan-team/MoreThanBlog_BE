using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Abstraction.Repository;
using Abstraction.Repository.Model;
using Abstraction.Service.Common;
using Abstraction.Service.Email;
using Abstraction.Service.UserService;
using Core.Constants.Enum;
using Core.Errors;
using Core.Helper;
using Core.Model.Common;
using Core.Model.Email;
using Core.Model.User;
using Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Service.UserService
{
    public class UserService: BaseService.Service, IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<UserOtpEntity> _userOtpRepository;

        private readonly ITokenService _tokenService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailService _emailService;
        private readonly SystemSetting _setting;

        public UserService(IUnitOfWork unitOfWork,
            AutoMapper.IMapper mapper,
            ITokenService tokenService,
            IEmailTemplateService emailTemplateService,
            IEmailService emailService,
            IOptions<SystemSetting> setting) : base(unitOfWork, mapper)
        {
            _userRepository = UnitOfWork.GetRepository<UserEntity>();
            _userOtpRepository = UnitOfWork.GetRepository<UserOtpEntity>();
            _tokenService = tokenService;
            _emailTemplateService = emailTemplateService;
            _emailService = emailService;
            _setting = setting.Value;
        }

        public async Task<JwtTokenResultModel> LoginAsync(LoginModel model, CancellationToken cancellationToken = default)
        {
            var userEntity = await _userRepository.Get(x => x.Email == model.Email)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(true);

            // Check User is exist
            if (userEntity == null)
            {
                throw new MoreThanBlogException(nameof(ErrorCode.UserNotFound), ErrorCode.UserNotFound);
            }

            // Compare password hash from request and database
            var passwordHash = PasswordHelper.HashPassword(model.Password, userEntity.PasswordLastUpdatedTime.UtcDateTime);
            if (passwordHash != userEntity.PasswordHash)
            {
                throw new MoreThanBlogException(nameof(ErrorCode.WrongPassword), ErrorCode.WrongPassword);
            }

            // Generate Access Token

            var now = DateTimeOffset.UtcNow;

            var claimIdentity = GenerateClaimsIdentity(userEntity);

            return await _tokenService.RequestTokenAsync(claimIdentity);
        }

        public async Task InitAdminAccountAsync(CancellationToken cancellation = default)
        {
            if (!_userRepository.Get().Any())
            {
                var now = DateTimeOffset.UtcNow;
                _userRepository.Add(new UserEntity
                {
                    FirstName = "More Than Blog",
                    LastName = "Admin",
                    Email = "morethan.team@yopmail.com",
                    IsActive = true,
                    PasswordLastUpdatedTime = now,
                    PasswordHash = PasswordHelper.HashPassword("abcd@1234", now)
                });

                await UnitOfWork.SaveChangesAsync(cancellation);
            }
        }

        public LoggedInUser GetUserProfile(string userId)
        {
            return  _userRepository.Get(x => x.Id == userId)
                .Select(x => _mapper.Map<LoggedInUser>(x))
                .FirstOrDefault();
        }

        public async Task<string> AddAsync(AddUserModel model, CancellationToken cancellationToken = default)
        {
            CheckDuplicateName(model.FirstName, model.LastName);

            var entity = _mapper.Map<UserEntity>(model);

            _userRepository.Add(entity);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            //Send email reset pass
            await SendEmailResetPasswordAsync(entity);

            return entity.Id;
        }

        public async Task UpdateAsync(string id, AddUserModel model, CancellationToken cancellationToken = default)
        {
            CheckExist(id);
            CheckDuplicateName(model.FirstName, model.LastName, id);

            var entity = _mapper.Map<UserEntity>(model);
            entity.Id = id;

            _userRepository.Update(entity, x => x.FirstName, x => x.Email,
                x=> x.IsActive, x => x.LastName, x => x.AvatarUrl, x => x.Phone);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<LoggedInUser> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _mapper.ProjectTo<LoggedInUser>(
                    _userRepository.Get(x => x.Id == id))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            _userRepository.DeleteWhere(x => x.Id == id);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<PagedResponseModel<LoggedInUser>> FilterAsync(FilterUserRequestModel model, CancellationToken cancellationToken = default)
        {
            var query = _userRepository.Get();

            if (!string.IsNullOrWhiteSpace(model.Terms))
            {
                query = query.Where(x => (x.FirstName + " " + x.LastName).Contains(model.Terms) || x.Email.Contains(model.Terms));
            }

            return new PagedResponseModel<LoggedInUser>
            {
                Total = await query.CountAsync(cancellationToken: cancellationToken),
                Items = await _mapper.ProjectTo<LoggedInUser>(query
                    .Skip(model.Skip)
                    .Take(model.Take)).ToListAsync(cancellationToken: cancellationToken)
            };
        }

        public async Task SetPasswordAsync(SetPasswordModel model, CancellationToken cancellationToken = default)
        {
            await CheckValidOtp(model.Otp, model.Email);

            var userEntity = await _userRepository.Get(x => x.Email == model.Email)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            var now = DateTimeOffset.UtcNow;

            // Set Password
            var newPasswordHash = PasswordHelper.HashPassword(model.NewPassword, now);
            userEntity.PasswordHash = newPasswordHash;
            userEntity.PasswordLastUpdatedTime = now;

            // Remove Set Password Token
            userEntity.EmailConfirmToken = null;
            userEntity.EmailConfirmTokenExpireTime = null;

            _userRepository.Update(
                userEntity,
                x => x.PasswordHash,
                x => x.PasswordLastUpdatedTime,
                x => x.EmailConfirmToken,
                x => x.EmailConfirmTokenExpireTime);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CheckValidOtp(string otp, string email)
        {
            var user = await _userOtpRepository.Get(x => x.Email == email).OrderByDescending(x => x.CreatedTime).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new MoreThanBlogException(nameof(ErrorCode.UserNotFound), ErrorCode.UserNotFound);
            }

            if (user.Otp != otp)
            {
                throw new MoreThanBlogException(nameof(ErrorCode.ConfirmEmailTokenInCorrect), ErrorCode.ConfirmEmailTokenInCorrect);
            }

            if (user.ExpireTime < DateTimeOffset.UtcNow)
            {
                throw new MoreThanBlogException(nameof(ErrorCode.ConfirmEmailTokenExpired), ErrorCode.ConfirmEmailTokenExpired);
            }
        }

        #region Utilities

        private ClaimsIdentity GenerateClaimsIdentity(UserEntity user)
        {
            var userClaims = new List<Claim>
            {
                new Claim(Core.Constants.ClaimTypes.UserId, user.Id.ToString()),
                new Claim(Core.Constants.ClaimTypes.Email, user.Email),
                new Claim(Core.Constants.ClaimTypes.UserName, user.FirstName + " " + user.LastName),
            };

            return new ClaimsIdentity(new GenericIdentity(user.Email, "Token"), userClaims);
        }

        private void CheckDuplicateName(string firstName, string lastName, string id = null)
        {
            var query = _userRepository.Get(x => x.FirstName == firstName && x.LastName == lastName);

            if (!string.IsNullOrWhiteSpace(id))
            {
                query = query.Where(x => x.Id != id);
            }

            if (query.Any())
            {
                throw new MoreThanBlogException(ErrorCode.DuplicateName);
            }
        }

        private async Task SendEmailResetPasswordAsync(UserEntity model)
        {
            var template = await _emailTemplateService.GetByKeyAsync(EmailTemplateType.InviteEmployee);

            if (template == null)
            {
                return;
            }
            var otp = PasswordHelper.GenerateOtp(TimeSpan.FromMinutes(5));

            _userOtpRepository.Add(new UserOtpEntity()
            {
                Email = model.Email,
                Otp = otp.Otp,
                ExpireTime = otp.ExpireTime
            });
            await UnitOfWork.SaveChangesAsync();

            var fullName = model.FirstName + " " + model.LastName;

            template.HtmlContent = template.HtmlContent.Replace("{fullName}", fullName)
                .Replace("{email}", model.Email)
                .Replace("{domain}", _setting.AdminSite)
                .Replace("{otp}", otp.Otp);

            await _emailService.SendAsync(new EmailSendModel
            {
                HtmlContent = template.HtmlContent,
                Subject = template.Title,
                To = model.Email
            });
        }

        private void CheckExist(string id)
        {
            if (!_userRepository.Get(x => x.Id == id).Any())
            {
                throw new MoreThanBlogException(nameof(ErrorCode.UserNotFound), ErrorCode.UserNotFound);
            }
        }

        #endregion
    }
}