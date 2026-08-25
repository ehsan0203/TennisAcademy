using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MTA.Application.DTOs.Auth;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using System.Security.Claims;

namespace MTA.Application.Services.Service;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly IRoleService _roleService;
    private readonly IValidator<RegisterDto> _registerValidator;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IConfiguration configuration,
        IRoleService roleService,
        IValidator<RegisterDto> registerValidator)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _configuration = configuration;
        _roleService = roleService;
        _registerValidator = registerValidator;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
    {
        var emailLower = loginDto.Email.ToLower();
        var account = await _unitOfWork.Repository<Account>()
            .GetQueryable()
            .AsNoTracking()
            .Include(a => a.Role)
            .Include(a => a.UserProfile)
                .ThenInclude(p => p.SkillLevel)
            .FirstOrDefaultAsync(a => a.Email.ToLower() == emailLower, ct);

        if (account == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, account.Password))
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!account.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated");

        var claims = BuildUserClaims(account);
        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(account.Id, refreshToken, ct);

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
                FirstName = profile?.FirstName ?? string.Empty,
                LastName = profile?.LastName ?? string.Empty,
                RoleId = account.RoleId,
                RoleTitle = account.Role?.Title ?? string.Empty,
                SkillLevelValue = profile?.SkillLevel?.Title ?? string.Empty,
                ImageUrl = account.MediaFile?.Url ?? string.Empty
            }
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
    {
        var validation = await _registerValidator.ValidateAsync(registerDto, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        if (await _unitOfWork.Repository<Account>().AnyAsync(a => a.Email.ToLower() == registerDto.Email.ToLower(), ct))
            throw new InvalidOperationException("Email already exists");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
        var studentRole = await _roleService.GetDefaultStudentRoleAsync(ct);

        var skillLevel = registerDto.SkillLevelId > 0
            ? await _unitOfWork.Repository<Level>().GetByIdAsync(registerDto.SkillLevelId, ct)
            : await _unitOfWork.Repository<Level>().GetByIdAsync(1, ct);

        if (skillLevel == null)
            throw new InvalidOperationException("Default skill level not found");

        var activeStatus = (await _unitOfWork.Repository<Lookup>()
            .GetAllAsync(l => l.Category == "AccountStatus" && l.Key.ToLower() == "active", ct))
            .FirstOrDefault();

        if (activeStatus == null)
            throw new InvalidOperationException("Active status not found");

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
        var account = new Account
        {
            Email = registerDto.Email,
            Password = hashedPassword,
            IsActive = true,
            RoleId = registerDto.RoleId == 0 ? studentRole.Id : registerDto.RoleId,
            StatusId = activeStatus.Id
        };
        await _unitOfWork.Repository<Account>().AddAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var userProfile = new UserProfile
        {
            AccountId = account.Id,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            DateOfBirth = registerDto.DateOfBirth,
            Experience = registerDto.Experience,
            SkillLevelId = registerDto.SkillLevelId == 0 ? skillLevel.Id : registerDto.SkillLevelId,
            HealthCondition = registerDto.HealthCondition,
            HealthDescription = registerDto.HealthDescription
        };
        await _unitOfWork.Repository<UserProfile>().AddAsync(userProfile, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitAsync(ct);

        var claims = BuildUserClaims(account);
        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();
        await SaveRefreshTokenAsync(account.Id, refreshToken, ct);

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
                RoleTitle = studentRole.Title,
                SkillLevelValue = skillLevel.Title,
                ImageUrl = string.Empty
            }
        };
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var tokenEntity = await _unitOfWork.Repository<RefreshToken>()
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked, ct);

        if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var account = await _unitOfWork.Repository<Account>()
            .GetQueryable()
            .AsNoTracking()
            .Include(a => a.Role)
            .Include(a => a.UserProfile)
                .ThenInclude(p => p.SkillLevel)
            .FirstOrDefaultAsync(a => a.Id == tokenEntity.AccountId, ct);

        if (account == null)
            throw new UnauthorizedAccessException("User not found");

        tokenEntity.IsRevoked = true;
        await _unitOfWork.Repository<RefreshToken>().UpdateAsync(tokenEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var claims = BuildUserClaims(account);
        var newAccessToken = _jwtService.GenerateAccessToken(claims);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        await SaveRefreshTokenAsync(account.Id, newRefreshToken, ct);

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
                FirstName = profile?.FirstName ?? string.Empty,
                LastName = profile?.LastName ?? string.Empty,
                RoleId = account.RoleId,
                SkillLevelId = profile?.SkillLevelId ?? 0,
                RoleTitle = account.Role?.Title ?? string.Empty,
                Experience = profile?.Experience ?? 0,
                SkillLevelValue = profile?.SkillLevel?.Title ?? string.Empty,
                ImageUrl = account.MediaFile?.Url ?? string.Empty
            }
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var tokenEntity = await _unitOfWork.Repository<RefreshToken>()
            .GetQueryable()
            .FirstOrDefaultAsync(t => t.Token == refreshToken, ct);

        if (tokenEntity == null) return false;

        tokenEntity.IsRevoked = true;
        await _unitOfWork.Repository<RefreshToken>().UpdateAsync(tokenEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
        => Task.FromResult(_jwtService.ValidateToken(token));

    private async Task SaveRefreshTokenAsync(int accountId, string token, CancellationToken ct)
    {
        await _unitOfWork.Repository<RefreshToken>().AddAsync(new RefreshToken
        {
            AccountId = accountId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        }, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static IEnumerable<Claim> BuildUserClaims(Account account) =>
    [
        new(ClaimTypes.NameIdentifier, account.Id.ToString()),
        new(ClaimTypes.Email, account.Email),
        new(ClaimTypes.Name, account.Email),
        new(ClaimTypes.Role, account.Role?.Title ?? "User"),
        new("UserId", account.Id.ToString()),
        new("RoleId", account.RoleId.ToString()),
        new("UserFullName", $"{account.UserProfile?.FirstName ?? ""} {account.UserProfile?.LastName ?? ""}".Trim()),
        new("AccountStatus", account.IsActive.ToString())
    ];
}
