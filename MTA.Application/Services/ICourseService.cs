using MTA.Application.DTOs;
using MTA.Application.DTOs.Course;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Course operations
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Get all courses with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title or description</param>
    /// <param name="levelId">Filter by level ID</param>
    /// <param name="statusId">Filter by status ID</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <returns>Paginated list of courses</returns>
    Task<PaginatedResult<CourseDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? levelId = null, int? statusId = null, decimal? minPrice = null, decimal? maxPrice = null);
    
    /// <summary>
    /// Get course by ID
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Course details</returns>
    Task<CourseDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get courses by level ID
    /// </summary>
    /// <param name="levelId">Level ID</param>
    /// <returns>List of courses</returns>
    Task<IEnumerable<CourseDto>> GetByLevelAsync(int levelId);
    
    /// <summary>
    /// Get courses by status ID
    /// </summary>
    /// <param name="statusId">Status ID</param>
    /// <returns>List of courses</returns>
    Task<IEnumerable<CourseDto>> GetByStatusAsync(int statusId);
    
    /// <summary>
    /// Create new course
    /// </summary>
    /// <param name="createCourseDto">Course creation data</param>
    /// <returns>Created course</returns>
    Task<CourseDto> CreateAsync(CreateCourseDto createCourseDto);
    
    /// <summary>
    /// Update existing course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="updateCourseDto">Updated course data</param>
    /// <returns>Updated course</returns>
    Task<CourseDto> UpdateAsync(int id, UpdateCourseDto updateCourseDto);
    
    /// <summary>
    /// Delete course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Change course status
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="statusId">New status ID</param>
    /// <returns>Updated course</returns>
    Task<CourseDto> ChangeStatusAsync(int id, int statusId);
    
    /// <summary>
    /// Change course level
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="levelId">New level ID</param>
    /// <returns>Updated course</returns>
    Task<CourseDto> ChangeLevelAsync(int id, int levelId);
    
    /// <summary>
    /// Update course price
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="price">New price</param>
    /// <returns>Updated course</returns>
    Task<CourseDto> UpdatePriceAsync(int id, decimal price);
    
    /// <summary>
    /// Get popular courses (by purchase count)
    /// </summary>
    /// <param name="count">Number of courses to return</param>
    /// <returns>List of popular courses</returns>
    Task<IEnumerable<CourseDto>> GetPopularCoursesAsync(int count = 10);
    
    /// <summary>
    /// Get free courses
    /// </summary>
    /// <returns>List of free courses</returns>
    Task<IEnumerable<CourseDto>> GetFreeCoursesAsync();
    
    /// <summary>
    /// Get courses with advanced filtering
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <returns>Filtered and paginated courses</returns>
    Task<PaginatedResult<CourseDto>> GetFilteredAsync(CourseFilterDto filter);
    
    /// <summary>
    /// Search courses by text
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Search results</returns>
    Task<PaginatedResult<CourseDto>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10);
    
    /// <summary>
    /// Get recommended courses for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="count">Number of recommendations</param>
    /// <returns>Recommended courses</returns>
    Task<IEnumerable<CourseDto>> GetRecommendedAsync(int userId, int count = 5);
    
    /// <summary>
    /// Get course statistics
    /// </summary>
    /// <returns>Course statistics</returns>
    Task<CourseStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Toggle course status (active/inactive)
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Updated course</returns>
    Task<CourseDto> ToggleStatusAsync(int id);
}
