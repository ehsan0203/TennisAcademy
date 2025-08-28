using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for UserProfile operations
/// </summary>
public interface IUserProfileService
{
    /// <summary>
    /// Get all user profiles with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for first name or last name</param>
    /// <param name="skillLevelId">Filter by skill level ID</param>
    /// <param name="minExperience">Minimum experience filter</param>
    /// <param name="maxExperience">Maximum experience filter</param>
    /// <returns>Paginated list of user profiles</returns>
    Task<PaginatedResult<UserProfileDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? skillLevelId = null, int? minExperience = null, int? maxExperience = null);
    
    /// <summary>
    /// Get user profile by ID
    /// </summary>
    /// <param name="id">User profile ID</param>
    /// <returns>User profile details</returns>
    Task<UserProfileDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get user profile by account ID
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <returns>User profile details</returns>
    Task<UserProfileDto?> GetByAccountIdAsync(int accountId);
    
    /// <summary>
    /// Get user profiles by skill level ID
    /// </summary>
    /// <param name="skillLevelId">Skill level ID</param>
    /// <returns>List of user profiles</returns>
    Task<IEnumerable<UserProfileDto>> GetBySkillLevelAsync(int skillLevelId);
    
    /// <summary>
    /// Get user profiles by experience range
    /// </summary>
    /// <param name="minExperience">Minimum experience</param>
    /// <param name="maxExperience">Maximum experience</param>
    /// <returns>List of user profiles</returns>
    Task<IEnumerable<UserProfileDto>> GetByExperienceRangeAsync(int minExperience, int maxExperience);
    
    /// <summary>
    /// Create new user profile
    /// </summary>
    /// <param name="userProfileDto">User profile data</param>
    /// <returns>Created user profile</returns>
    Task<UserProfileDto> CreateAsync(UserProfileDto userProfileDto);
    
    /// <summary>
    /// Update existing user profile
    /// </summary>
    /// <param name="id">User profile ID</param>
    /// <param name="userProfileDto">Updated user profile data</param>
    /// <returns>Updated user profile</returns>
    Task<UserProfileDto> UpdateAsync(int id, UserProfileDto userProfileDto);
    
    /// <summary>
    /// Delete user profile
    /// </summary>
    /// <param name="id">User profile ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Update user profile experience
    /// </summary>
    /// <param name="id">User profile ID</param>
    /// <param name="experience">New experience value</param>
    /// <returns>Updated user profile</returns>
    Task<UserProfileDto> UpdateExperienceAsync(int id, int experience);
    
    /// <summary>
    /// Update user profile skill level
    /// </summary>
    /// <param name="id">User profile ID</param>
    /// <param name="skillLevelId">New skill level ID</param>
    /// <returns>Updated user profile</returns>
    Task<UserProfileDto> UpdateSkillLevelAsync(int id, int skillLevelId);
    
    /// <summary>
    /// Get user profile statistics
    /// </summary>
    /// <returns>User profile statistics</returns>
    Task<UserProfileStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get user profiles by date of birth range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of user profiles</returns>
    Task<IEnumerable<UserProfileDto>> GetByDateOfBirthRangeAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// User profile statistics DTO
/// </summary>
public class UserProfileStatisticsDto
{
    public int TotalProfiles { get; set; }
    public int ProfilesWithExperience { get; set; }
    public int ProfilesWithoutExperience { get; set; }
    public double AverageExperience { get; set; }
    public Dictionary<int, int> ProfilesPerSkillLevel { get; set; } = new();
    public int ProfilesThisMonth { get; set; }
    public int ProfilesLastMonth { get; set; }
    public int ProfilesByAgeGroup18_25 { get; set; }
    public int ProfilesByAgeGroup26_35 { get; set; }
    public int ProfilesByAgeGroup36_45 { get; set; }
    public int ProfilesByAgeGroup46_55 { get; set; }
    public int ProfilesByAgeGroup55Plus { get; set; }
}
