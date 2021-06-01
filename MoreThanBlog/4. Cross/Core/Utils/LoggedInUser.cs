using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Core.Utils
{
    public class LoggedInUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User;

        public LoggedInUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public LoggedInUser()
        {
            throw new NotImplementedException();
        }

        public static LoggedInUser Current
        {
            get => Get();
            set => Set(value);
        }

        private static LoggedInUser Get()
        {
            return null; //todo
        }

        private static void Set(LoggedInUser value)
        {
            //todo
        }

        // Properties

        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarUrl { get; set; }
        public string FullName => FirstName + " " + LastName;

        // Audit
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}