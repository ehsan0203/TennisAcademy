using MTA.Application.DTOs.User;

namespace MTA.Application.Services;

/// <summary>
/// Optimized service interface for UserProfile operations
/// </summary>
public interface IUserProfileService
{
    #region CRUD

    Task<UserProfileDto?> GetByIdAsync(int id);               
    Task<UserProfileDto?> GetByAccountIdAsync(int accountId);  
    Task<UserProfileDto> CreateAsync(UserProfileDto userProfileDto); 
    Task<UserProfileDto?> UpdateAsync(int id, UpdateUserProfileDto updateDto); 
    Task<bool> DeleteAsync(int id);                              

    #endregion

    #region Partial Updates

    Task<UserProfileDto?> UpdateAvatarAsync(int id, string imageUrl);
    Task<UserProfileDto?> UpdateSkillLevelAsync(int id, int skillLevelId);
    Task<UserProfileDto?> UpdateExperienceAsync(int id, int experience);
    Task<PagedResult<UserProfileDto>> QueryAsync(UserSearchDto queryDto);
    #endregion

}
