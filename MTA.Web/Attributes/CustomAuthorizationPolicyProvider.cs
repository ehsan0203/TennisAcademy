using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MTA.Web.Attributes;

/// <summary>
/// Custom authorization policy provider for role-based policies
/// </summary>
public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if policy name starts with "Role"
        if (policyName.StartsWith("Role", StringComparison.OrdinalIgnoreCase))
        {
            // Extract roles from policy name (e.g., "RoleAdmin" -> ["Admin"])
            var roleName = policyName.Substring(4); // Remove "Role" prefix
            
            if (!string.IsNullOrEmpty(roleName))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new RoleRequirement(roleName));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }
        }

        // Check if policy name starts with "Roles"
        if (policyName.StartsWith("Roles", StringComparison.OrdinalIgnoreCase))
        {
            // Extract roles from policy name (e.g., "RolesAdminModerator" -> ["Admin", "Moderator"])
            var rolesString = policyName.Substring(5); // Remove "Roles" prefix
            
            if (!string.IsNullOrEmpty(rolesString))
            {
                // Split by capital letters to get individual roles
                var roles = SplitByCapitalLetters(rolesString);
                var policy = new AuthorizationPolicyBuilder();
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new RoleRequirement(roles));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }
        }

        // Fallback to default policy provider
        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    /// <summary>
    /// Splits a string by capital letters (e.g., "AdminModerator" -> ["Admin", "Moderator"])
    /// </summary>
    /// <param name="input">Input string</param>
    /// <returns>Array of strings split by capital letters</returns>
    private static string[] SplitByCapitalLetters(string input)
    {
        var result = new List<string>();
        var currentWord = "";

        foreach (var c in input)
        {
            if (char.IsUpper(c) && !string.IsNullOrEmpty(currentWord))
            {
                result.Add(currentWord);
                currentWord = "";
            }
            currentWord += c;
        }

        if (!string.IsNullOrEmpty(currentWord))
        {
            result.Add(currentWord);
        }

        return result.ToArray();
    }
}
