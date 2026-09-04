using MTA.Application.DTOs;
using MTA.Application.DTOs.Course;

namespace MTA.Application.Services;

public interface ICourseService
{
    /// <param name="includeUnpublished">
    /// Admin-only. The public catalogue must show published courses alone — a draft or
    /// an archived course still belongs to whoever bought it, but nobody else should
    /// be able to find it, let alone buy it.
    /// </param>
    Task<PaginatedResult<CourseDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? levelId = null, int? statusId = null, decimal? minPrice = null, decimal? maxPrice = null, bool includeUnpublished = false, CancellationToken ct = default);
    Task<CourseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<CourseDto>> GetByLevelAsync(int levelId, CancellationToken ct = default);
    Task<IEnumerable<CourseDto>> GetByStatusAsync(int statusId, CancellationToken ct = default);
    Task<CourseDto> CreateAsync(CreateCourseDto createCourseDto, CancellationToken ct = default);
    Task<CourseDto> UpdateAsync(int id, UpdateCourseDto updateCourseDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<CourseDto> ChangeStatusAsync(int id, int statusId, CancellationToken ct = default);
    Task<CourseDto> ChangeLevelAsync(int id, int levelId, CancellationToken ct = default);
    Task<CourseDto> UpdatePriceAsync(int id, decimal price, CancellationToken ct = default);
    Task<IEnumerable<CourseDto>> GetPopularCoursesAsync(int count = 10, CancellationToken ct = default);
    Task<IEnumerable<CourseDto>> GetFreeCoursesAsync(CancellationToken ct = default);
    Task<PaginatedResult<CourseDto>> GetFilteredAsync(CourseFilterDto filter, CancellationToken ct = default);
    Task<PaginatedResult<CourseDto>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10, CancellationToken ct = default);
    Task<IEnumerable<CourseDto>> GetRecommendedAsync(int userId, int count = 5, CancellationToken ct = default);
    Task<CourseStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    Task<CourseDto> ToggleStatusAsync(int id, CancellationToken ct = default);
}
