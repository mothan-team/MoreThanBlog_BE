using System.Threading;
using System.Threading.Tasks;
using Core.Model.Email;

namespace Abstraction.Service.Email
{
    public interface IEmailService
    {
        Task SendAsync(EmailSendModel model, CancellationToken cancellationToken = default);
    }
}