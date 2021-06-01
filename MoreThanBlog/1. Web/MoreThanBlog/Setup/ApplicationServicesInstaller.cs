using Abstraction.Service.Common;
using Abstraction.Service.UserService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Common;
using Service.UserService;

namespace MoreThanBlog.Setup
{
    public static class ApplicationServicesInstaller
    {
        public static void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITokenService, JwtTokenService>();
            services.AddTransient<IUserService, UserService>();
        }
    }
}