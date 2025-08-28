using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MTA.Application.Services;
using System.Security.Claims;

namespace MTA.Web.Attributes;

/// <summary>
/// Custom authorization handler for role-based access control
/// </summary>
public class CustomAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly IJwtService _jwtService;

    public CustomAuthorizationHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        RoleRequirement requirement)
    {
        // Check if user is authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        // Get user role from claims
        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(userRole))
        {
            return Task.CompletedTask;
        }

        // Check if user has required role
        if (requirement.Roles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Custom requirement for role-based authorization
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    public string[] Roles { get; }

    public RoleRequirement(params string[] roles)
    {
        Roles = roles;
    }
}
