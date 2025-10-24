using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MTA.Application.Services;

/// <summary>
/// JWT service implementation
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    /// <summary>
    /// Generates a JWT access token
    /// </summary>
    /// <param name="claims">User claims</param>
    /// <returns>JWT token string</returns>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"])),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = credentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates a JWT refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Extracts claims from a JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Collection of claims</returns>
    public IEnumerable<Claim> GetClaimsFromToken(string token)
    {
        var tokenValidationParameters = GetTokenValidationParameters();
        var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
        return principal.Claims;
    }

    /// <summary>
    /// Gets user ID from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID as string</returns>
    public string? GetUserIdFromToken(string token)
    {
        try
        {
            var claims = GetClaimsFromToken(token);
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "UserId")?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets user email from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User email as string</returns>
    public string? GetUserEmailFromToken(string token)
    {
        try
        {
            var claims = GetClaimsFromToken(token);
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets user role from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User role as string</returns>
    public string? GetUserRoleFromToken(string token)
    {
        try
        {
            var claims = GetClaimsFromToken(token);
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets all user information from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Dictionary containing user information</returns>
    public Dictionary<string, string> GetUserInfoFromToken(string token)
    {
        try
        {
            var claims = GetClaimsFromToken(token);
            var userInfo = new Dictionary<string, string>();
            
            foreach (var claim in claims)
            {
                if (!userInfo.ContainsKey(claim.Type))
                {
                    userInfo[claim.Type] = claim.Value;
                }
            }
            
            return userInfo;
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if valid</returns>
    public bool ValidateToken(string token)
    {
        try
        {
            var tokenValidationParameters = GetTokenValidationParameters();
            _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the expiration time of a token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Expiration time</returns>
    public DateTime GetTokenExpirationTime(string token)
    {
        var tokenValidationParameters = GetTokenValidationParameters();
        var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
        return validatedToken.ValidTo;
    }

    /// <summary>
    /// Gets token validation parameters
    /// </summary>
    /// <returns>Token validation parameters</returns>
    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!)),
            ValidateIssuer = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}
