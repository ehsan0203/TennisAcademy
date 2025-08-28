using System.Security.Claims;

namespace MTA.Application.Services;

/// <summary>
/// Interface for JWT service
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT access token
    /// </summary>
    /// <param name="claims">User claims</param>
    /// <returns>JWT token string</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// Generates a JWT refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Extracts claims from a JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Collection of claims</returns>
    IEnumerable<Claim> GetClaimsFromToken(string token);

    /// <summary>
    /// Gets user ID from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID as string</returns>
    string? GetUserIdFromToken(string token);

    /// <summary>
    /// Gets user email from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User email as string</returns>
    string? GetUserEmailFromToken(string token);

    /// <summary>
    /// Gets user role from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User role as string</returns>
    string? GetUserRoleFromToken(string token);

    /// <summary>
    /// Gets all user information from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Dictionary containing user information</returns>
    Dictionary<string, string> GetUserInfoFromToken(string token);

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if valid</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Gets the expiration time of a token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Expiration time</returns>
    DateTime GetTokenExpirationTime(string token);
}
