using System;

namespace Abstraction.Repository.Model
{
    public class UserOtpEntity : MoreThanBlogEntity
    {
        public string Email { get; set; }

        public string Otp { get; set; }

        public DateTimeOffset? ExpireTime { get; set; }
    }
}