using System;

namespace Core.Model.Common
{
    public class OtpModel
    {
        public string Otp { get; set; }
        public DateTimeOffset ExpireTime { get; set; }
    }
}