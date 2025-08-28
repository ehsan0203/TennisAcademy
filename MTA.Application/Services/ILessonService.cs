using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Lesson operations
/// </summary>
public interface ILessonService
{
    /// <summary>
    /// Get all lessons with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title or description</param>
    /// <param name="courseId">Filter by course ID</param>
    /// <param name="isFree">Filter by free status</param>
    /// <returns>Paginated list of lessons</returns>
    Task<PaginatedResult<LessonDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? courseId = null, bool? isFree = null);
    
    /// <summary>
    /// Get lesson by ID
    /// </summary>
    /// <param name="id">Lesson ID</param>
    /// <returns>Lesson details</returns>
    Task<LessonDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get lessons by course ID
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of lessons</returns>
    Task<IEnumerable<LessonDto>> GetByCourseAsync(int courseId);
    
    /// <summary>
    /// Get free lessons
    /// </summary>
    /// <returns>List of free lessons</returns>
    Task<IEnumerable<LessonDto>> GetFreeLessonsAsync();
    
    /// <summary>
    /// Get paid lessons
    /// </summary>
    /// <returns>List of paid lessons</returns>
    Task<IEnumerable<LessonDto>> GetPaidLessonsAsync();
    
    /// <summary>
    /// Create new lesson
    /// </summary>
    /// <param name="lessonDto">Lesson data</param>
    /// <returns>Created lesson</returns>
    Task<LessonDto> CreateAsync(LessonDto lessonDto);
    
    /// <summary>
    /// Update existing lesson
    /// </summary>
    /// <param name="id">Lesson ID</param>
    /// <param name="lessonDto">Updated lesson data</param>
    /// <returns>Updated lesson</returns>
    Task<LessonDto> UpdateAsync(int id, LessonDto lessonDto);
    
    /// <summary>
    /// Delete lesson
    /// </summary>
    /// <param name="id">Lesson ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Change lesson free status
    /// </summary>
    /// <param name="id">Lesson ID</param>
    /// <param name="isFree">Free status</param>
    /// <returns>Updated lesson</returns>
    Task<LessonDto> SetFreeStatusAsync(int id, bool isFree);
    
    /// <summary>
    /// Change lesson course
    /// </summary>
    /// <param name="id">Lesson ID</param>
    /// <param name="courseId">New course ID</param>
    /// <returns>Updated lesson</returns>
    Task<LessonDto> ChangeCourseAsync(int id, int courseId);
    
    /// <summary>
    /// Update lesson order
    /// </summary>
    /// <param name="id">Lesson ID</param>
    /// <param name="order">New order</param>
    /// <returns>Updated lesson</returns>
    Task<LessonDto> UpdateOrderAsync(int id, int order);
    
    /// <summary>
    /// Get lesson statistics
    /// </summary>
    /// <returns>Lesson statistics</returns>
    Task<LessonStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get lessons by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of lessons</returns>
    Task<IEnumerable<LessonDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Lesson statistics DTO
/// </summary>
public class LessonStatisticsDto
{
    public int TotalLessons { get; set; }
    public int FreeLessons { get; set; }
    public int PaidLessons { get; set; }
    public int LessonsWithMedia { get; set; }
    public double AverageMediaFilesPerLesson { get; set; }
    public int LessonsThisMonth { get; set; }
    public int LessonsLastMonth { get; set; }
}
