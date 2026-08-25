using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs.User;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for UserProfile operations (light & optimized)
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(IUnitOfWork unitOfWork, ILogger<UserProfileService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    #region CRUD

    public async Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var userProfile = await _unitOfWork.UserProfiles.GetQueryable()
                .AsNoTracking()
                .Include(up => up.Account)
                .ThenInclude(a => a.Role)
                .Include(up => up.Account)
                .ThenInclude(a => a.Status)
                .Include(up => up.SkillLevel)
                .FirstOrDefaultAsync(up => up.Id == id, ct);

            return userProfile != null ? MapToDto(userProfile) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<UserProfileDto?> GetByAccountIdAsync(int accountId, CancellationToken ct = default)
    {
        try
        {
            var userProfile = await _unitOfWork.UserProfiles.GetQueryable()
                .AsNoTracking()
                .Include(up => up.Account)
                .ThenInclude(a => a.Role)
                .Include(up => up.Account)
                .ThenInclude(a => a.Status)
                .Include(up => up.SkillLevel)
                .FirstOrDefaultAsync(up => up.AccountId == accountId, ct);

            return userProfile != null ? MapToDto(userProfile) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile by account ID: {AccountId}", accountId);
            throw;
        }
    }

    public async Task<UserProfileDto> CreateAsync(UserProfileDto userProfileDto, CancellationToken ct = default)
    {
        try
        {
            var userProfile = new UserProfile
            {
                FirstName = userProfileDto.FirstName,
                LastName = userProfileDto.LastName,
                DateOfBirth = userProfileDto.DateOfBirth,
                Experience = userProfileDto.Experience,
                AccountId = userProfileDto.AccountId,
                SkillLevelId = userProfileDto.SkillLevelId
            };

            await _unitOfWork.UserProfiles.AddAsync(userProfile, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await GetByIdAsync(userProfile.Id, ct) ?? userProfileDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user profile");
            throw;
        }
    }

    public async Task<PagedResult<UserProfileDto>> QueryAsync(UserSearchDto queryDto, CancellationToken ct = default)
    {
        var query = _unitOfWork.UserProfiles.GetQueryable()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(queryDto.SearchTerm))
        {
            query = query.Where(u =>
                u.FirstName.Contains(queryDto.SearchTerm) ||
                u.LastName.Contains(queryDto.SearchTerm));
        }

        if (queryDto.SkillLevelId.HasValue)
        {
            query = query.Where(u => u.SkillLevelId == queryDto.SkillLevelId.Value);
        }

        if (queryDto.Experience.HasValue)
        {
            query = query.Where(u => u.Experience == queryDto.Experience.Value);
        }

        if (queryDto.IsActive.HasValue)
        {
            query = query.Where(u => u.Account.IsActive == queryDto.IsActive.Value);
        }

        int total = await query.CountAsync(ct);

        var items = await query
            .Include(u => u.Account)
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(u => new UserProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Account.Email,
                DateOfBirth = u.DateOfBirth,
                Experience = u.Experience,
                RoleId = u.Account.RoleId,
                RoleTitle = u.Account.Role.Title,
                StatusId = u.Account.StatusId,
                StatusValue = u.Account.Status.Value,
                SkillLevelId = u.SkillLevelId,
                SkillLevelValue = u.SkillLevel != null ? u.SkillLevel.Title : null,
                AccountId = u.AccountId
            })
            .ToListAsync(ct);

        return new PagedResult<UserProfileDto>
        {
            Items = items,
            Total = total,
            Page = queryDto.Page,
            PageSize = queryDto.PageSize
        };
    }


    public async Task<UserProfileDto?> UpdateAsync(int id, UpdateUserProfileDto dto, CancellationToken ct = default)
    {
        var user = await _unitOfWork.UserProfiles.GetByIdAsync(id, ct);
        if (user is null) return null;

        Account? account = null;
        var shouldUpdateAccount = false;

        if (dto.StatusId.HasValue || dto.RoleId.HasValue)
        {
            account = await _unitOfWork.Accounts.GetByIdAsync(user.AccountId, ct);
        }

        if (account is not null)
        {
            if (dto.StatusId.HasValue && account.StatusId != dto.StatusId.Value)
            {
                account.StatusId = dto.StatusId.Value;
                shouldUpdateAccount = true;
            }

            if (dto.RoleId.HasValue && account.RoleId != dto.RoleId.Value)
            {
                account.RoleId = dto.RoleId.Value;
                shouldUpdateAccount = true;
            }

            if (shouldUpdateAccount)
            {
                await _unitOfWork.Accounts.UpdateAsync(account, ct);
            }
        }

        if (dto.FirstName != null)
            user.FirstName = dto.FirstName.Trim();

        if (dto.LastName != null)
            user.LastName = dto.LastName.Trim();

        if (dto.DateOfBirth.HasValue)
            user.DateOfBirth = dto.DateOfBirth.Value; // یا .Date اگر فقط تاریخ مهمه

        if (dto.Experience.HasValue)
            user.Experience = dto.Experience.Value;

        if (dto.SkillLevelId.HasValue)
            user.SkillLevelId = dto.SkillLevelId.Value;

        if (dto.HealthCondition.HasValue)
        {
            user.HealthCondition = dto.HealthCondition.Value;
            user.HealthDescription = dto.HealthCondition.Value
                ? (dto.HealthDescription ?? user.HealthDescription) // اگر توضیح جدید آمد جایگزین کن
                : null; // اگر وضعیت سلامت false شد، توضیح هم پاک شود
        }
        else if (dto.HealthDescription != null && user.HealthCondition)
        {
            // وقتی فقط توضیح آمد و وضعیت سلامت قبلاً true بوده
            user.HealthDescription = dto.HealthDescription;
        }

        await _unitOfWork.UserProfiles.UpdateAsync(user, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(id, ct);
            if (userProfile == null)
                return false;

            await _unitOfWork.UserProfiles.DeleteAsync(userProfile.Id, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user profile with ID: {UserId}", id);
            throw;
        }
    }

    #endregion

    #region Partial Updates

    public async Task<UserProfileDto?> UpdateAvatarAsync(int id, string imageUrl, CancellationToken ct = default)
    {
        try
        {
            var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(id, ct);
            if (userProfile == null)
                return null;

            // Update the account image
            var account = await _unitOfWork.Accounts.GetByIdAsync(userProfile.AccountId, ct);
            if (account != null)
            {
                account.MediaFile.Url = imageUrl;
                await _unitOfWork.Accounts.UpdateAsync(account, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return await GetByIdAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user avatar with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<UserProfileDto?> UpdateSkillLevelAsync(int id, int skillLevelId, CancellationToken ct = default)
    {
        try
        {
            var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(id, ct);
            if (userProfile == null)
                return null;

            userProfile.SkillLevelId = skillLevelId;

            await _unitOfWork.UserProfiles.UpdateAsync(userProfile, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await GetByIdAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user skill level with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<UserProfileDto?> UpdateExperienceAsync(int id, int experience, CancellationToken ct = default)
    {
        try
        {
            var userProfile = await _unitOfWork.UserProfiles.GetByIdAsync(id, ct);
            if (userProfile == null)
                return null;

            userProfile.Experience = experience;

            await _unitOfWork.UserProfiles.UpdateAsync(userProfile, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await GetByIdAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user experience with ID: {UserId}", id);
            throw;
        }
    }

    #endregion

    #region Helper Methods

    private UserProfileDto MapToDto(UserProfile userProfile)
    {
        return new UserProfileDto
        {
            Id = userProfile.Id,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            Email = userProfile.Account?.Email ?? string.Empty,
            DateOfBirth = userProfile.DateOfBirth,
            Experience = userProfile.Experience,
            AccountId = userProfile.AccountId,
            RoleId = userProfile.Account?.RoleId ?? 0,
            RoleTitle = userProfile.Account?.Role?.Title,
            StatusId = userProfile.Account?.StatusId ?? 0,
            StatusValue = userProfile.Account?.Status?.Value,
            SkillLevelId = userProfile.SkillLevelId,
            SkillLevelValue = userProfile.SkillLevel?.Title,
            CreatedAt = userProfile.CreatedAt,
            UpdatedAt = userProfile.UpdatedAt
        };
    }

    #endregion
}
