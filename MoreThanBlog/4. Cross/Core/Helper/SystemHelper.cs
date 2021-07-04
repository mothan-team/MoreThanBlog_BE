using Microsoft.AspNetCore.Http;

namespace Core.Helper
{
    public static class SystemHelper
    {
        public static string GetDomain(this HttpRequest request)
        {
            return request.Scheme + "://" + request.Host.Value;
        }
    }
}