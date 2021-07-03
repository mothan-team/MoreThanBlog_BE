using Core.Constants.Enum;

namespace Abstraction.Repository.Model
{
    public class EmailTemplateEntity : MoreThanBlogEntity
    {
        public string Title { get; set; }
        public string HtmlContent { get; set; }
        public EmailTemplateType Key { get; set; }
    }
}