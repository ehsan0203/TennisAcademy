using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface ILessonService
{
    Task<PaginatedResult<LessonDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? courseId = null, bool? isFree = null, CancellationToken ct = default);
    Task<LessonDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<LessonDto>> GetByCourseAsync(int courseId, CancellationToken ct = default);
    Task<LessonDto> CreateAsync(CreateLessonDto lessonDto, CancellationToken ct = default);
    Task<LessonDto?> UpdateAsync(int id, UpdateLessonDto lessonDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<LessonDto> ChangeCourseAsync(int id, int courseId, CancellationToken ct = default);
    Task<LessonDto> UpdateOrderAsync(int id, int order, CancellationToken ct = default);
}
