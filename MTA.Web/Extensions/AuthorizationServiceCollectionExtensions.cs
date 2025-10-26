using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MTA.Web.Attributes;

namespace MTA.Web.Extensions;

/// <summary>
/// Extension methods for registering authorization services and policies.
/// </summary>
public static class AuthorizationServiceCollectionExtensions
{
    /// <summary>
    /// Configures the application's role-based authorization infrastructure.
    /// </summary>
    /// <param name="services">The dependency injection container.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRoleBasedAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

        services.AddAuthorization();

        return services;
    }
}
