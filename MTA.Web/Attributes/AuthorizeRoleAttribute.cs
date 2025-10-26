using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace MTA.Web.Attributes;

/// <summary>
/// Provides a strongly typed helper attribute for registering role-based authorization policies.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class AuthorizeRoleAttribute : AuthorizeAttribute
{
    public AuthorizeRoleAttribute(params string[] roles)
    {
        ArgumentNullException.ThrowIfNull(roles);

        var sanitizedRoles = roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (sanitizedRoles.Length == 0)
        {
            throw new ArgumentException("At least one valid role must be specified.", nameof(roles));
        }

        Policy = BuildPolicyName(sanitizedRoles);
    }

    [return: NotNull]
    private static string BuildPolicyName(string[] roles)
    {
        if (roles.Length == 1)
        {
            return $"Role{roles[0]}";
        }

        return $"Roles{string.Concat(roles)}";
    }
}
