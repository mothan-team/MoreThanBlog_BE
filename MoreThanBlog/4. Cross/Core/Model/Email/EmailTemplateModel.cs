using System;
using Core.Constants.Enum;

namespace Core.Model.Email
{
    public class EmailTemplateModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string HtmlContent { get; set; }
        public EmailTemplateType Key { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}