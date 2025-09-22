using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MTA.Application.DTOs.Auth;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using FluentValidation;

namespace MTA.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly RoleService _roleService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IConfiguration configuration,
        RoleService roleService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _configuration = configuration;
        _roleService = roleService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var account = (await _unitOfWork.Repository<Account>()
            .GetAllAsync(include: q => q.Include(a => a.Role)
                                        .Include(a => a.UserProfile)
                                        .ThenInclude(p => p.SkillLevel)))
            .FirstOrDefault(a => a.Email.ToLower() == loginDto.Email.ToLower());

        if (account == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, account.Password))
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!account.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated");

        var claims = GenerateUserClaims(account);
        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(account.Id, refreshToken);

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
                ImageUrl = account.MediaFile.Url
            }
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // 1️⃣ ولیدیشن
        var validator = new RegisterDtoValidator();
        var validationResult = validator.Validate(registerDto);
        if (!validationResult.IsValid)
            throw new FluentValidation.ValidationException(validationResult.Errors);

        var accountRepo = _unitOfWork.Repository<Account>();
        if (await accountRepo.AnyAsync(a => a.Email.ToLower() == registerDto.Email.ToLower()))
            throw new InvalidOperationException("Email already exists");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var studentRole = await _roleService.GetDefaultStudentRoleAsync();

        var skillRepo = _unitOfWork.Repository<Level>();
        var skillLevel = registerDto.SkillLevelId > 0
            ? await skillRepo.GetByIdAsync(registerDto.SkillLevelId)
            : await skillRepo.GetByIdAsync(1);

        if (skillLevel == null)
            throw new InvalidOperationException("Default skill level not found");

        var lookupRepo = _unitOfWork.Repository<Lookup>();
        var statuses = await lookupRepo.GetAllAsync();
        var activeStatus = statuses.FirstOrDefault(l => l.Category == "AccountStatus" && l.Key.ToLower() == "active");
        if (activeStatus == null)
            throw new InvalidOperationException("Active status not found");

        var account = new Account
        {
            Email = registerDto.Email,
            Password = hashedPassword,
            IsActive = true,
            RoleId = registerDto.RoleId==0 ? studentRole.Id : registerDto.RoleId,
            StatusId = activeStatus.Id
        };
        await accountRepo.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        var userProfile = new UserProfile
        {
            AccountId = account.Id,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            DateOfBirth = registerDto.DateOfBirth,
            Experience = registerDto.Experience,
            SkillLevelId = registerDto.SkillLevelId==0 ? skillLevel.Id : registerDto.SkillLevelId,
            HealthCondition = registerDto.HealthCondition,
            HealthDescription = registerDto.HealthDescription
        };
        await _unitOfWork.Repository<UserProfile>().AddAsync(userProfile);
        await _unitOfWork.SaveChangesAsync();

        var claims = GenerateUserClaims(account);
        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();
        await SaveRefreshTokenAsync(account.Id, refreshToken);

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
                RoleTitle = studentRole?.Title ?? "",
                SkillLevelValue = skillLevel?.Title ?? "",
                ImageUrl = account.MediaFile.Url
            }
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var tokenRepo = _unitOfWork.Repository<RefreshToken>();
        var tokenEntity = (await tokenRepo.GetAllAsync())
                          .FirstOrDefault(t => t.Token == refreshToken);

        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(tokenEntity.AccountId);
        if (account == null)
            throw new UnauthorizedAccessException("User not found");

        var claims = GenerateUserClaims(account);
        var newAccessToken = _jwtService.GenerateAccessToken(claims);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Revoke old token
        tokenEntity.IsRevoked = true;
        await _unitOfWork.SaveChangesAsync();

        // Save new token
        await SaveRefreshTokenAsync(account.Id, newRefreshToken);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"]) * 60,
            User = new UserInfoDto
            {
                Id = account.Id,
                Email = account.Email,
                FirstName = account.UserProfile?.FirstName ?? "",
                LastName = account.UserProfile?.LastName ?? "",
                RoleTitle = account.Role?.Title ?? "",
                SkillLevelValue = account.UserProfile?.SkillLevel?.Title ?? "",
                ImageUrl = account.MediaFile.Url
            }
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var tokenRepo = _unitOfWork.Repository<RefreshToken>();
        var tokenEntity = (await tokenRepo.GetAllAsync())
                          .FirstOrDefault(t => t.Token == refreshToken);

        if (tokenEntity == null)
            return false;

        tokenEntity.IsRevoked = true;
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        return _jwtService.ValidateToken(token);
    }

    private async Task SaveRefreshTokenAsync(int userId, string refreshToken)
    {
        await _unitOfWork.Repository<RefreshToken>().AddAsync(new RefreshToken
        {
            AccountId = userId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
        await _unitOfWork.SaveChangesAsync();
    }

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
            new("AccountStatus", account.IsActive.ToString())
        };

        return claims;
    }
}
