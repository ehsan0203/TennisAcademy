using MTA.Application.Services;
using MTA.Domain.Interfaces;
using MTA.Infrastructure.Repositories;

namespace MTA.Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IFAQService, FAQService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }

}
