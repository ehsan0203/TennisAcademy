using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface ILevelService
{
    Task<PaginatedResult<LevelDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default);
    Task<LevelDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<LevelDto?> GetByTitleAsync(string title, CancellationToken ct = default);
    Task<LevelDto> CreateAsync(LevelDto levelDto, CancellationToken ct = default);
    Task<LevelDto> UpdateAsync(int id, LevelDto levelDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<LevelStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    Task<IEnumerable<LevelDto>> GetLevelsWithCourseCountAsync(CancellationToken ct = default);
    Task<IEnumerable<LevelDto>> GetLevelsWithUserCountAsync(CancellationToken ct = default);
    Task<IEnumerable<LevelDto>> GetLevelsOrderedByDifficultyAsync(CancellationToken ct = default);
    Task<IEnumerable<LevelDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}

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
