using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstraction.Repository;
using Abstraction.Repository.Model;
using Abstraction.Service.Email;
using AutoMapper;
using Core.Constants.Enum;
using Core.Model.Email;
using Microsoft.EntityFrameworkCore;

namespace Service.Email
{
    public class EmailTemplateService : BaseService.Service, IEmailTemplateService
    {
        private readonly IRepository<EmailTemplateEntity> _emailTemplateRepository;

        public EmailTemplateService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _emailTemplateRepository = unitOfWork.GetRepository<EmailTemplateEntity>();
        }

        public async Task InitialEmailTemplate(CancellationToken cancellationToken = default)
        {
            var entitiesToAdd = new List<EmailTemplateEntity>
            {
                new EmailTemplateEntity
                {
                    Key = EmailTemplateType.InviteEmployee,
                    Title = "Welcome to More Than Team Blog - Create your password!",
                    HtmlContent = "<p>Dear {fullName}, </p>" +
                              "<p>You are receiving this message following your account has been created in More Than Blog Admin.<br/>" +
                              "Please click <a href=\"{domain}/me/set-pass/{email}/{otp}\">here</a> to reset your password. <br/>" +
                              "Please click <a href=\"{domain}\">here</a> to access More Than Blog Admin platform.</p>" +
                              "<p>Thanks, <br/> More Than Blog</p>"
                }
            };

            var existedEmailTemplateKeys = await _emailTemplateRepository.Get()
                .Select(x => x.Key)
                .ToArrayAsync(cancellationToken: cancellationToken);

            _emailTemplateRepository.AddRange(entitiesToAdd.Where(x => existedEmailTemplateKeys.All(y => y != x.Key))
                .ToArray());

            await UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<EmailTemplateModel> GetByKeyAsync(EmailTemplateType key, CancellationToken cancellationToken = default)
        {
            return await _mapper.ProjectTo<EmailTemplateModel>(
                    _emailTemplateRepository.Get(x => x.Key == key))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}