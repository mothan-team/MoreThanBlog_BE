namespace Core.Model.Email
{
    public class EmailModel
    {
        public string SenderEmailAddress { get; set; }

        public string SenderDisplayName { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string HtmlContent { get; set; }

        public string ContentLength { get; set; }
    }
}