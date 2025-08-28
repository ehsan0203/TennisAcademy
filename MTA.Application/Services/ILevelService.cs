using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Level operations
/// </summary>
public interface ILevelService
{
    /// <summary>
    /// Get all levels with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title</param>
    /// <returns>Paginated list of levels</returns>
    Task<PaginatedResult<LevelDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null);
    
    /// <summary>
    /// Get level by ID
    /// </summary>
    /// <param name="id">Level ID</param>
    /// <returns>Level details</returns>
    Task<LevelDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get level by title
    /// </summary>
    /// <param name="title">Level title</param>
    /// <returns>Level details</returns>
    Task<LevelDto?> GetByTitleAsync(string title);
    
    /// <summary>
    /// Create new level
    /// </summary>
    /// <param name="levelDto">Level data</param>
    /// <returns>Created level</returns>
    Task<LevelDto> CreateAsync(LevelDto levelDto);
    
    /// <summary>
    /// Update existing level
    /// </summary>
    /// <param name="id">Level ID</param>
    /// <param name="levelDto">Updated level data</param>
    /// <returns>Updated level</returns>
    Task<LevelDto> UpdateAsync(int id, LevelDto levelDto);
    
    /// <summary>
    /// Delete level
    /// </summary>
    /// <param name="id">Level ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Get level statistics
    /// </summary>
    /// <returns>Level statistics</returns>
    Task<LevelStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get levels with course count
    /// </summary>
    /// <returns>List of levels with course counts</returns>
    Task<IEnumerable<LevelDto>> GetLevelsWithCourseCountAsync();
    
    /// <summary>
    /// Get levels with user count
    /// </summary>
    /// <returns>List of levels with user counts</returns>
    Task<IEnumerable<LevelDto>> GetLevelsWithUserCountAsync();
    
    /// <summary>
    /// Get levels ordered by difficulty
    /// </summary>
    /// <returns>List of levels ordered by difficulty</returns>
    Task<IEnumerable<LevelDto>> GetLevelsOrderedByDifficultyAsync();
    
    /// <summary>
    /// Get levels by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of levels</returns>
    Task<IEnumerable<LevelDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Level statistics DTO
/// </summary>
public class LevelStatisticsDto
{
    public int TotalLevels { get; set; }
    public int LevelsWithCourses { get; set; }
    public int LevelsWithoutCourses { get; set; }
    public int LevelsWithUsers { get; set; }
    public int LevelsWithoutUsers { get; set; }
    public double AverageCoursesPerLevel { get; set; }
    public double AverageUsersPerLevel { get; set; }
    public int LevelsThisMonth { get; set; }
    public int LevelsLastMonth { get; set; }
    public Dictionary<int, int> CoursesPerLevel { get; set; } = new();
    public Dictionary<int, int> UsersPerLevel { get; set; } = new();
}
