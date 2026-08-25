using FluentValidation;
using MTA.Application.Services;
using MTA.Application.Services.Interface;
using MTA.Application.Services.Service;
using MTA.Application.Validators;
using MTA.Domain.Interfaces;
using MTA.Infrastructure.Interceptors;
using MTA.Infrastructure.Repositories;
using MTA.Web.Services;

namespace MTA.Web;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Audit interceptor (scoped so it can access ICurrentUser per-request)
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<AuditInterceptor>();

        // Services
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IFAQService, FAQService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<ILevelService, LevelService>();
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<IMappingService, MappingService>();
        services.AddScoped<IMediaFileService, MediaFileService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IPackageHistoryService, PackageHistoryService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IUserCourseHistoryService, UserCourseHistoryService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IPaymentService, SquarePaymentService>();
        services.AddScoped<IStatisticsService, StatisticsService>();

        // UoW — registered here; Persistence layer also registers it, last registration wins (fine)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // FluentValidation — scan Application assembly once for all AbstractValidator<T>
        services.AddValidatorsFromAssemblyContaining<CreateCourseDtoValidator>();

        return services;
    }
}
