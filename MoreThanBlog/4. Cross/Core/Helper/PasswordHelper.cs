using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Core.Model.Common;
using Core.Utils;

namespace Core.Helper
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password, DateTimeOffset passwordLastUpdatedTime)
        {
            if (string.IsNullOrWhiteSpace(password)) return null;

            var salt = GenerateSalt(passwordLastUpdatedTime);

            var passwordHash = HashPassword(password, salt);

            return passwordHash;
        }

        public static string GenerateSalt(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString("ddMMyyyyhhmmss");
        }

        public static string HashPassword(string password, string salt, int iterations = 100000)
        {
            var valueBytes = Encoding.UTF8.GetBytes(password);

            var saltBytes = Encoding.UTF8.GetBytes(salt);

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(valueBytes, saltBytes, iterations))
            {
                var hashBytes = rfc2898DeriveBytes.GetBytes(32);

                var hashString = Convert.ToBase64String(hashBytes);

                return hashString;
            }
        }

        public static OtpModel GenerateOtp(TimeSpan? timeSpan)
        {
            if (!timeSpan.HasValue) timeSpan = new TimeSpan(0, 1,0,0);

            var result = new OtpModel();
            //Generate otp

            var random = new Random();
            const string chars = "123456789";

            result.Otp = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            result.ExpireTime = DateTimeOffset.UtcNow.Add(timeSpan.Value);
            return result;
        }
    }
}