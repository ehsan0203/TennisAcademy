using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MTA.Application.DTOs.Auth;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Authentication response with tokens</returns>
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Find user by email
        var account = (await _unitOfWork.Repository<Account>()
            .GetAllAsync(include: q => q.Include(a => a.Role).Include(a => a.UserProfile).ThenInclude(p => p.SkillLevel)))
            .FirstOrDefault(a => a.Email.ToLower() == loginDto.Email.ToLower());


        if (account == null)
            throw new UnauthorizedAccessException("Invalid email or password");

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, account.Password))
            throw new UnauthorizedAccessException("Invalid email or password");

        // Check if account is active
        if (!account.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated");

        // Generate tokens
        var claims = GenerateUserClaims(account);
        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Get user profile info
        var profile = account.UserProfile;

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"]) * 60,
            User = new UserInfoDto
            {
                Id = account.Id,
                Email = account.Email,
                FirstName = profile?.FirstName ?? "",
                LastName = profile?.LastName ?? "",
                RoleId = account.RoleId,
                RoleTitle = account.Role?.Title ?? "",
                SkillLevelValue = profile?.SkillLevel?.Title ?? "", 
                ImageUrl = account.Image
            }
        };
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerDto">Registration data</param>
    /// <returns>Authentication response with tokens</returns>
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if email already exists
        var accounts = await _unitOfWork.Repository<Account>().GetAllAsync();
        if (accounts.Any(a => a.Email.ToLower() == registerDto.Email.ToLower()))
            throw new InvalidOperationException("Email already exists");

        // Hash password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // Get default role (Student)
        var roles = await _unitOfWork.Repository<Role>().GetAllAsync();
        var studentRole = roles.FirstOrDefault(r => r.Title.ToLower() == "student");
        if (studentRole == null)
            throw new InvalidOperationException("Default role not found");

        // Get default skill level from Level table (Beginner = Id 1)
        var beginnerSkill = await _unitOfWork.Repository<Level>().GetByIdAsync(1);
        if (beginnerSkill == null)
            throw new InvalidOperationException("Default skill level not found");


        // Get active account status from Lookup
        var statuses = await _unitOfWork.Repository<Lookup>().GetAllAsync();
        var activeStatus = statuses.FirstOrDefault(l => l.Category == "AccountStatus" && l.Key.ToLower() == "active");
        if (activeStatus == null)
            throw new InvalidOperationException("Active status not found");

        // Create account with ProfileId
        var account = new Account
        {
            Email = registerDto.Email,
            Password = hashedPassword,
            IsActive = true,
            RoleId = studentRole.Id,
            StatusId = activeStatus.Id
        };
        await _unitOfWork.Repository<Account>().AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        // Create UserProfile first
        var userProfile = new UserProfile
        {
            AccountId = account.Id,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            DateOfBirth = DateTime.Now.AddYears(-18),
            Experience = 0,
            SkillLevelId = beginnerSkill.Id
        };
        await _unitOfWork.Repository<UserProfile>().AddAsync(userProfile);
        await _unitOfWork.SaveChangesAsync();

        // Generate tokens
        var claims = GenerateUserClaims(account);
        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"]) * 60,
            User = new UserInfoDto
            {
                Id = account.Id,
                Email = account.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                RoleTitle = account.Role?.Title ?? "",
                SkillLevelValue = userProfile.SkillLevel?.Title ?? "",
                ImageUrl = account.Image
            }
        };
    }

    /// <summary>
    /// Refreshes an expired access token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New authentication response</returns>
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // In a real application, you'd validate the refresh token against a database
        // For now, we'll just generate a new token
        // This is a simplified implementation

        // Extract user ID from refresh token (you'd typically store this in a database)
        // For now, we'll throw an exception
        throw new NotImplementedException("Refresh token functionality requires database implementation");
    }

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <returns>True if successful</returns>
    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        // In a real application, you'd invalidate the refresh token in a database
        // For now, we'll return true
        return true;
    }

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if valid</returns>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        return _jwtService.ValidateToken(token);
    }

    /// <summary>
    /// Generates user claims for JWT token
    /// </summary>
    /// <param name="account">User account</param>
    /// <returns>Collection of claims</returns>
    private static IEnumerable<Claim> GenerateUserClaims(Account account)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Email, account.Email),
            new(ClaimTypes.Name, account.Email), 
            new(ClaimTypes.Role, account.Role?.Title ?? "User"),
            new("UserId", account.Id.ToString()),
            new("RoleId", account.RoleId.ToString()),
            new("UserFullName", $"{account.UserProfile?.FirstName ?? ""} {account.UserProfile?.LastName ?? ""}".Trim()),
            new("AccountStatus", account.IsActive.ToString()),
            new("StatusId", account.StatusId.ToString()),
            new("ImageUrl", account.Image ?? ""),
            new("SkillLevel", account.UserProfile?.SkillLevel?.Title ?? ""),
            new("Experience", (account.UserProfile?.Experience ?? 0).ToString())
        };

        return claims;
    }
}
