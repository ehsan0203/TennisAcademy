using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IUserCourseHistoryService
{
    Task<PaginatedResult<UserCourseHistoryDetailDto>> GetAllAsync(int page = 1, int pageSize = 10, int? accountId = null, int? courseId = null, CancellationToken ct = default);
    Task<UserCourseHistoryDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<UserCourseHistoryDetailDto>> GetByAccountAsync(int accountId, CancellationToken ct = default);
    Task<IEnumerable<UserCourseHistoryDetailDto>> GetByCourseAsync(int courseId, CancellationToken ct = default);
    Task<bool> UserHasPurchasedCourseAsync(int accountId, int courseId, CancellationToken ct = default);
    Task<UpdateUserCourseHistoryDto> CreateAsync(CreateUserCourseHistoryDto userCourseHistoryDto, CancellationToken ct = default);
    Task<UpdateUserCourseHistoryDto> UpdateAsync(int id, UpdateUserCourseHistoryDto userCourseHistoryDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<UserCourseHistoryStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    Task<IEnumerable<UserCourseHistoryDetailDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}

public class UserCourseHistoryStatisticsDto
{
    public int TotalPurchases { get; set; }
    public decimal TotalRevenue { get; set; }
    public int UniqueUsers { get; set; }
    public int UniqueCourses { get; set; }
    public double AverageCoursesPerUser { get; set; }
    public double AverageRevenuePerUser { get; set; }
    public int PurchasesThisMonth { get; set; }
    public int PurchasesLastMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
}

public class UserLearningProgressDto
{
    public int AccountId { get; set; }
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public int TotalCoursesPurchased { get; set; }
    public int TotalCoursesCompleted { get; set; }
    public int TotalLessonsCompleted { get; set; }
    public decimal TotalSpent { get; set; }
    public double CompletionRate { get; set; }
    public DateTime LastActivityDate { get; set; }
    public List<CourseProgressDto> CourseProgress { get; set; } = new();
}

public class CourseProgressDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public double ProgressPercentage { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? LastAccessDate { get; set; }
}
