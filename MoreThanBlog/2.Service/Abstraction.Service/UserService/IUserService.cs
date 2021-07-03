using Core.Model.User;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.Common;
using Core.Utils;

namespace Abstraction.Service.UserService
{
    public interface IUserService
    {
        Task<JwtTokenResultModel> LoginAsync(LoginModel model, CancellationToken cancellationToken = default);

        Task InitAdminAccountAsync(CancellationToken cancellation = default);

        LoggedInUser GetUserProfile(string userId);

        Task<string> AddAsync(AddUserModel model, CancellationToken cancellationToken = default);

        Task UpdateAsync(string id, AddUserModel model, CancellationToken cancellationToken = default);

        Task<LoggedInUser> GetAsync(string id, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);

        Task<PagedResponseModel<LoggedInUser>> FilterAsync(FilterUserRequestModel model, CancellationToken cancellationToken = default);

        Task SetPasswordAsync(SetPasswordModel model, CancellationToken cancellationToken = default);

        Task CheckValidOtp(string otp, string email);
    }
}