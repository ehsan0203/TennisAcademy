using MTA.Application.DTOs;
using MTA.Application.DTOs.User;

namespace MTA.Application.Services;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<UserProfileDto?> GetByAccountIdAsync(int accountId, CancellationToken ct = default);
    Task<UserProfileDto> CreateAsync(UserProfileDto userProfileDto, CancellationToken ct = default);
    Task<UserProfileDto?> UpdateAsync(int id, UpdateUserProfileDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<UserProfileDto?> UpdateAvatarAsync(int id, string imageUrl, CancellationToken ct = default);
    Task<UserProfileDto?> UpdateSkillLevelAsync(int id, int skillLevelId, CancellationToken ct = default);
    Task<UserProfileDto?> UpdateExperienceAsync(int id, int experience, CancellationToken ct = default);
    Task<PagedResult<UserProfileDto>> QueryAsync(UserSearchDto queryDto, CancellationToken ct = default);
}
