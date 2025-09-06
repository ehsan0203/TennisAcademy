using FluentValidation;
using MTA.Application.Services;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using MTA.Infrastructure.Repositories;

namespace MTA.Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IFAQService, FAQService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<IMappingService, MappingService>();
            services.AddScoped<IMediaFileService, MediaFileService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IPackageHistoryService, PackageHistoryService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<IUserCourseHistoryService,UserCourseHistoryService>();
            services.AddScoped<IVideoContentService, VideoContentService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<RoleService>();
            services.AddScoped<IValidator<IEnumerable<Role>>, GetStudentRoleValidator>();

            return services;
        }
    }
}
