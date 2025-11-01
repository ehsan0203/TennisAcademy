using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
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
    private readonly IEmailService _emailService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IConfiguration configuration,
        RoleService roleService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _configuration = configuration;
        _roleService = roleService;
        _emailService = emailService;
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
                ImageUrl = string.IsNullOrEmpty(account.MediaFile?.Url) ? string.Empty : account.MediaFile.Url
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
                ImageUrl = string.IsNullOrEmpty(account.MediaFile?.Url) ? string.Empty : account.MediaFile.Url
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

        // بارگذاری Account همراه با Role و UserProfile و SkillLevel
        var account = (await _unitOfWork.Repository<Account>()
                        .GetAllAsync(include: q => q
                            .Include(a => a.Role)
                            .Include(a => a.UserProfile)
                                .ThenInclude(p => p.SkillLevel)))
                      .FirstOrDefault(a => a.Id == tokenEntity.AccountId);

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

        var profile = account.UserProfile;

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"]) * 60,
            User = new UserInfoDto
            {
                Id = account.Id,
                Email = account.Email,
                FirstName = profile?.FirstName ?? "",
                LastName = profile?.LastName ?? "",
                RoleId = account.RoleId,
                SkillLevelId = profile.SkillLevelId,
                RoleTitle = account.Role?.Title ?? "",
                Experience = profile.Experience,
                SkillLevelValue = profile?.SkillLevel?.Title ?? "",
                ImageUrl = string.IsNullOrEmpty(account.MediaFile?.Url) ? string.Empty : account.MediaFile.Url
            }
        };
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto requestDto)
    {
        var accountRepository = _unitOfWork.Repository<Account>();
        var normalizedEmail = requestDto.Email.ToLower();
        var account = await accountRepository.FirstOrDefaultAsync(a => a.Email.ToLower() == normalizedEmail);

        if (account == null)
        {
            throw new KeyNotFoundException("Account with the provided email was not found.");
        }

        var newPassword = GenerateSecurePassword();
        account.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        account.UpdatedAt = DateTime.UtcNow;

        await accountRepository.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendPasswordResetAsync(account.Email, newPassword);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(int accountId, ResetPasswordRequestDto requestDto)
    {
        var accountRepository = _unitOfWork.Repository<Account>();
        var account = await accountRepository.GetByIdAsync(accountId);

        if (account == null)
        {
            throw new KeyNotFoundException("Account with the provided identifier was not found.");
        }

        if (!BCrypt.Net.BCrypt.Verify(requestDto.CurrentPassword, account.Password))
        {
            throw new InvalidOperationException("The current password is incorrect.");
        }

        if (BCrypt.Net.BCrypt.Verify(requestDto.NewPassword, account.Password))
        {
            throw new InvalidOperationException("The new password must be different from the current password.");
        }

        account.Password = BCrypt.Net.BCrypt.HashPassword(requestDto.NewPassword);
        account.UpdatedAt = DateTime.UtcNow;

        await accountRepository.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static string GenerateSecurePassword(int length = 12)
    {
        const string allowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%^&*";
        Span<byte> randomBytes = stackalloc byte[length];
        RandomNumberGenerator.Fill(randomBytes);

        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            chars[i] = allowedCharacters[randomBytes[i] % allowedCharacters.Length];
        }

        return new string(chars);
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
