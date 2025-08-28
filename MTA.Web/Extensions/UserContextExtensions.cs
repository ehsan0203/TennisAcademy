using System.Security.Claims;

namespace MTA.Web.Extensions;

/// <summary>
/// Extension methods for accessing user information from HttpContext
/// </summary>
public static class UserContextExtensions
{
    /// <summary>
    /// Gets current user ID from HttpContext
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>User ID as string</returns>
    public static string? GetCurrentUserId(this HttpContext httpContext)
    {
        return httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Gets current user email from HttpContext
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>User email as string</returns>
    public static string? GetCurrentUserEmail(this HttpContext httpContext)
    {
        return httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets current user role from HttpContext
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>User role as string</returns>
    public static string? GetCurrentUserRole(this HttpContext httpContext)
    {
        return httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
    }

    /// <summary>
    /// Gets current user full name from HttpContext
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>User full name as string</returns>
    public static string? GetCurrentUserFullName(this HttpContext httpContext)
    {
        return httpContext.User.FindFirst("UserFullName")?.Value;
    }

    /// <summary>
    /// Gets current user skill level from HttpContext
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>User skill level as string</returns>
    public static string? GetCurrentUserSkillLevel(this HttpContext httpContext)
    {
        return httpContext.User.FindFirst("SkillLevel")?.Value;
    }

    /// <summary>
    /// Gets current user experience from HttpContext
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>User experience as string</returns>
    public static string? GetCurrentUserExperience(this HttpContext httpContext)
    {
        return httpContext.User.FindFirst("Experience")?.Value;
    }

    /// <summary>
    /// Gets current user image URL from HttpContext
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>User image URL as string</returns>
    public static string? GetCurrentUserImageUrl(this HttpContext httpContext)
    {
        return httpContext.User.FindFirst("ImageUrl")?.Value;
    }

    /// <summary>
    /// Gets current user account status from HttpContext
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>User account status as string</returns>
    public static string? GetCurrentUserAccountStatus(this HttpContext httpContext)
    {
        return httpContext.User.FindFirst("AccountStatus")?.Value;
    }

    /// <summary>
    /// Checks if current user has a specific role
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="role">Role to check</param>
    /// <returns>True if user has the role</returns>
    public static bool HasRole(this HttpContext httpContext, string role)
    {
        var userRole = httpContext.GetCurrentUserRole();
        return !string.IsNullOrEmpty(userRole) && 
               string.Equals(userRole, role, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if current user has any of the specified roles
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="roles">Roles to check</param>
    /// <returns>True if user has any of the roles</returns>
    public static bool HasAnyRole(this HttpContext httpContext, params string[] roles)
    {
        var userRole = httpContext.GetCurrentUserRole();
        if (string.IsNullOrEmpty(userRole))
            return false;

        return roles.Any(role => string.Equals(userRole, role, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if current user is authenticated
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>True if user is authenticated</returns>
    public static bool IsAuthenticated(this HttpContext httpContext)
    {
        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Gets all user claims as a dictionary
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>Dictionary containing all user claims</returns>
    public static Dictionary<string, string> GetAllUserClaims(this HttpContext httpContext)
    {
        var claims = new Dictionary<string, string>();
        
        foreach (var claim in httpContext.User.Claims)
        {
            if (!claims.ContainsKey(claim.Type))
            {
                claims[claim.Type] = claim.Value;
            }
        }
        
        return claims;
    }
}
