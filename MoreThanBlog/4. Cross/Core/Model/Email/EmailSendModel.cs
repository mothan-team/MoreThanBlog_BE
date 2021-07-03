using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Core.Model.Email
{
    public class EmailSendModel
    {
        public string SenderDisplayName { get; set; }

        public string To { get; set; }

        public List<string> ToRange { get; set; }

        public string Subject { get; set; }

        public string HtmlContent { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedByAppId { get; set; }

        public List<IFormFile> UploadFile { get; set; }
    }
}