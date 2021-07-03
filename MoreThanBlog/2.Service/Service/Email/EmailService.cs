using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstraction.Repository;
using Abstraction.Service.Email;
using AutoMapper;
using Core.Model.Email;
using Core.Utils;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Service.Email
{
    public class EmailService : BaseService.Service , IEmailService
    {
        private readonly SmtpConfig _stmpConfig;

        public EmailService(IUnitOfWork unitOfWork, 
            IMapper mapper,
            IOptions<SmtpConfig> stmpConfig) : base(unitOfWork, mapper)
        {
            _stmpConfig = stmpConfig.Value;
        }

        public async Task SendAsync(EmailSendModel model, CancellationToken cancellationToken = default)
        {
            model.SenderDisplayName = string.IsNullOrWhiteSpace(model.SenderDisplayName)
                ? _stmpConfig.Name
                : model.SenderDisplayName;

            // Send Email
            await SendBySmtp(model, cancellationToken).ConfigureAwait(true);
        }

        private async Task SendBySmtp(EmailSendModel model, CancellationToken cancellationToken)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(model.SenderDisplayName, _stmpConfig.EmailAddress));

            if (model.ToRange is null)
            {
                model.ToRange = new List<string>() { model.To };
            }

            foreach (var emailTo in model.ToRange)
            {
                emailMessage.To.Add(new MailboxAddress(emailTo, emailTo));
            }

            emailMessage.Subject = model.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = model.HtmlContent;

            if (model.UploadFile != null && model.UploadFile.Any())
            {
                foreach (var file in model.UploadFile)
                {
                    await using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        builder.Attachments.Add(fileName: file.FileName, data: fileBytes);
                    }
                }
            }

            emailMessage.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            // Convert SAS SecureSocketOption to Mail kit SecureSocketOptions
            var secureSocketOptions = (SecureSocketOptions)((int)_stmpConfig.SecureSocketOption);

            await client.ConnectAsync(_stmpConfig.Host, _stmpConfig.Port,
                secureSocketOptions, cancellationToken).ConfigureAwait(true);

            await client.AuthenticateAsync(_stmpConfig.Username,
                _stmpConfig.Password, cancellationToken).ConfigureAwait(true);

            await client.SendAsync(emailMessage, cancellationToken).ConfigureAwait(true);
        }
    }
}