using System.Threading;
using System.Threading.Tasks;
using Core.Constants.Enum;
using Core.Model.Email;

namespace Abstraction.Service.Email
{
    public interface IEmailTemplateService
    {
        Task InitialEmailTemplate(CancellationToken cancellationToken = default);

        Task<EmailTemplateModel> GetByKeyAsync(EmailTemplateType key, CancellationToken cancellationToken = default);
    }
}