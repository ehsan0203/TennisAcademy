using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for UserCourseHistory operations
/// </summary>
public interface IUserCourseHistoryService
{
    /// <summary>
    /// Get all user course histories with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="accountId">Filter by account ID</param>
    /// <param name="courseId">Filter by course ID</param>
    /// <returns>Paginated list of user course histories</returns>
    Task<PaginatedResult<UserCourseHistoryDto>> GetAllAsync(int page = 1, int pageSize = 10, int? accountId = null, int? courseId = null);
    
    /// <summary>
    /// Get user course history by ID
    /// </summary>
    /// <param name="id">User course history ID</param>
    /// <returns>User course history details</returns>
    Task<UserCourseHistoryDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get user course histories by account ID
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <returns>List of user course histories</returns>
    Task<IEnumerable<UserCourseHistoryDto>> GetByAccountAsync(int accountId);
    
    /// <summary>
    /// Get user course histories by course ID
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of user course histories</returns>
    Task<IEnumerable<UserCourseHistoryDto>> GetByCourseAsync(int courseId);
    
    /// <summary>
    /// Check if user has purchased course
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <param name="courseId">Course ID</param>
    /// <returns>True if user has purchased course</returns>
    Task<bool> UserHasPurchasedCourseAsync(int accountId, int courseId);
    
    /// <summary>
    /// Create new user course history
    /// </summary>
    /// <param name="userCourseHistoryDto">User course history data</param>
    /// <returns>Created user course history</returns>
    Task<UserCourseHistoryDto> CreateAsync(UserCourseHistoryDto userCourseHistoryDto);
    
    /// <summary>
    /// Update existing user course history
    /// </summary>
    /// <param name="id">User course history ID</param>
    /// <param name="userCourseHistoryDto">Updated user course history data</param>
    /// <returns>Updated user course history</returns>
    Task<UserCourseHistoryDto> UpdateAsync(int id, UserCourseHistoryDto userCourseHistoryDto);
    
    /// <summary>
    /// Delete user course history
    /// </summary>
    /// <param name="id">User course history ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Get user course history statistics
    /// </summary>
    /// <returns>User course history statistics</returns>
    Task<UserCourseHistoryStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get user course histories by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of user course histories</returns>
    Task<IEnumerable<UserCourseHistoryDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    ///// <summary>
    ///// Get popular courses (by purchase count)
    ///// </summary>
    ///// <param name="count">Number of courses to return</param>
    ///// <returns>List of popular courses</returns>
    //Task<IEnumerable<CourseDto>> GetPopularCoursesAsync(int count = 10);
    
    ///// <summary>
    ///// Get user learning progress
    ///// </summary>
    ///// <param name="accountId">Account ID</param>
    ///// <returns>User learning progress</returns>
    //Task<UserLearningProgressDto> GetUserLearningProgressAsync(int accountId);
}

/// <summary>
/// User course history statistics DTO
/// </summary>
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

/// <summary>
/// User learning progress DTO
/// </summary>
public class UserLearningProgressDto
{
    public int AccountId { get; set; }
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public int TotalCoursesPurchased { get; set; }
    public int TotalCoursesCompleted { get; set; }
    public int TotalLessonsCompleted { get; set; }
    public decimal TotalSpent { get; set; }
    public double CompletionRate { get; set; } // percentage
    public DateTime LastActivityDate { get; set; }
    public List<CourseProgressDto> CourseProgress { get; set; } = new();
}

/// <summary>
/// Course progress DTO
/// </summary>
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
