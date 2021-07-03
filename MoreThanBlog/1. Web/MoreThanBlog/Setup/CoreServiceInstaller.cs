using Abstraction.Repository;
using Core.Utils;
using Core.Validator;
using FluentValidation.AspNetCore;
using Mapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoreThanBlog.Filter;
using Repository;
using IValidator = Core.Validator.IValidator;

namespace MoreThanBlog.Setup
{
    public class CoreServiceInstaller
    {
        public static void ConfigureCoreServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddAutoMapper(typeof(IMapper).Assembly);

            services.AddMvcCore(
                    options =>
                    {
                        options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                    }
                )
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining(typeof(IValidator))); ;

            services.Configure<SmtpConfig>(configuration.GetSection(nameof(SmtpConfig)));
            services.Configure<SystemSetting>(configuration.GetSection(nameof(SystemSetting)));

            services.AddDbContext<DbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork<DbContext>>();
            services.AddScoped<ModelValidationFilterAttribute>();
            services.AddScoped<SystemApiAuthFilter>();


            services.AddMemoryCache();// Todo research
        }
    }
}