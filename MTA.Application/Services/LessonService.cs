using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for Lesson operations
/// </summary>
public class LessonService : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LessonService> _logger;
    private readonly IMapper _mapper;


    public LessonService(IUnitOfWork unitOfWork, ILogger<LessonService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all lessons with optional filtering
    /// </summary>
    public async Task<PaginatedResult<LessonDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? courseId = null, bool? isFree = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Lesson>().GetQueryable()
                .Include(l => l.Course)
                .Include(l => l.MediaFiles)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(l => 
                    l.Title.Contains(searchTerm) || 
                    (l.Description != null && l.Description.Contains(searchTerm)));
            }

            if (courseId.HasValue)
            {
                query = query.Where(l => l.CourseId == courseId.Value);
            }

            if (isFree.HasValue)
            {
                query = query.Where(l => l.IsFree == isFree.Value);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination and ordering
            var lessons = await query
                .OrderBy(l => l.CourseId)
                .ThenBy(l => l.Order)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to DTOs
            var lessonDtos = lessons.Select(MapToDto).ToList();

            return new PaginatedResult<LessonDto>
            {
                Data = lessonDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lessons with page {Page}, pageSize {PageSize}, searchTerm {SearchTerm}, courseId {CourseId}, isFree {IsFree}", 
                page, pageSize, searchTerm, courseId, isFree);
            throw;
        }
    }

    /// <summary>
    /// Get lesson by ID
    /// </summary>
    public async Task<LessonDto?> GetByIdAsync(int id)
    {
        try
        {
            var lesson = await _unitOfWork.Repository<Lesson>().GetQueryable()
                .Include(l => l.Course)
                .Include(l => l.MediaFiles)
                .FirstOrDefaultAsync(l => l.Id == id);

            return lesson != null ? MapToDto(lesson) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lesson with ID: {LessonId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get lessons by course ID
    /// </summary>
    public async Task<IEnumerable<LessonDto>> GetByCourseAsync(int courseId)
    {
        try
        {
            var lessons = await _unitOfWork.Repository<Lesson>().GetQueryable()
                .Include(l => l.Course)
                .Include(l => l.MediaFiles)
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Order)
                .ToListAsync();

            return lessons.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lessons by course ID: {CourseId}", courseId);
            throw;
        }
    }

    /// <summary>
    /// Create new lesson
    /// </summary>
    public async Task<LessonDto> CreateAsync(CreateLessonDto lessonDto)
    {
        try
        {
            // Validate course exists
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(lessonDto.CourseId);
            if (course == null)
            {
                throw new InvalidOperationException($"Course with ID {lessonDto.CourseId} not found");
            }

            // Get the next order number for the course
            var maxOrder = await _unitOfWork.Repository<Lesson>().GetQueryable()
                .Where(l => l.CourseId == lessonDto.CourseId)
                .MaxAsync(l => (int?)l.Order) ?? 0;

            var lesson = new Lesson
            {
                Title = lessonDto.Title,
                Description = lessonDto.Description,
                IsFree = lessonDto.IsFree,
                CourseId = lessonDto.CourseId,
                Order = maxOrder + 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Lesson>().AddAsync(lesson);
            await _unitOfWork.SaveChangesAsync();

            // Map directly to LessonDto (using AutoMapper or manual mapping)
            return _mapper.Map<LessonDto>(lesson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lesson. Title: {Title}, CourseId: {CourseId}", lessonDto.Title, lessonDto.CourseId);
            throw;
        }
    }


    /// <summary>
    /// Update existing lesson
    /// </summary>
    public async Task<LessonDto> UpdateAsync(int id, LessonDto lessonDto)
    {
        try
        {
            var lesson = await _unitOfWork.Repository<Lesson>().GetByIdAsync(id);
            if (lesson == null)
            {
                throw new InvalidOperationException($"Lesson with ID {id} not found");
            }

            // Validate course exists if changing course
            if (lessonDto.CourseId != lesson.CourseId)
            {
                var course = await _unitOfWork.Repository<Course>().GetByIdAsync(lessonDto.CourseId);
                if (course == null)
                {
                    throw new InvalidOperationException($"Course with ID {lessonDto.CourseId} not found");
                }
            }

            // Update properties
            lesson.Title = lessonDto.Title;
            lesson.Description = lessonDto.Description;
            lesson.IsFree = lessonDto.IsFree;
            lesson.CourseId = lessonDto.CourseId;
            lesson.Order = lessonDto.Order;
            lesson.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Lesson>().UpdateAsync(lesson);
            await _unitOfWork.SaveChangesAsync();

            // Return the updated lesson
            return await GetByIdAsync(id) ?? lessonDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lesson with ID: {LessonId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete lesson
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var lesson = await _unitOfWork.Repository<Lesson>().GetByIdAsync(id);
            if (lesson == null)
            {
                return false;
            }

            // Check if lesson has media files
            var hasMediaFiles = await _unitOfWork.Repository<MediaFile>().GetQueryable()
                .AnyAsync(mf => mf.LessonId == id);

            if (hasMediaFiles)
            {
                throw new InvalidOperationException($"Cannot delete lesson '{lesson.Title}' as it has associated media files");
            }

            await _unitOfWork.Repository<Lesson>().DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lesson with ID: {LessonId}", id);
            throw;
        }
    }

    /// <summary>
    /// Change lesson course
    /// </summary>
    public async Task<LessonDto> ChangeCourseAsync(int id, int courseId)
    {
        try
        {
            var lesson = await _unitOfWork.Repository<Lesson>().GetByIdAsync(id);
            if (lesson == null)
            {
                throw new InvalidOperationException($"Lesson with ID {id} not found");
            }

            // Validate new course exists
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(courseId);
            if (course == null)
            {
                throw new InvalidOperationException($"Course with ID {courseId} not found");
            }

            lesson.CourseId = courseId;
            lesson.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Lesson>().UpdateAsync(lesson);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id) ?? throw new InvalidOperationException("Failed to retrieve updated lesson");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing course for lesson with ID: {LessonId}", id);
            throw;
        }
    }

    /// <summary>
    /// Update lesson order
    /// </summary>
    public async Task<LessonDto> UpdateOrderAsync(int id, int order)
    {
        try
        {
            var lesson = await _unitOfWork.Repository<Lesson>().GetByIdAsync(id);
            if (lesson == null)
            {
                throw new InvalidOperationException($"Lesson with ID {id} not found");
            }

            lesson.Order = order;
            lesson.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Lesson>().UpdateAsync(lesson);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id) ?? throw new InvalidOperationException("Failed to retrieve updated lesson");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order for lesson with ID: {LessonId}", id);
            throw;
        }
    }
    #region Helper Methods

    /// <summary>
    /// Map Lesson entity to LessonDto
    /// </summary>
    private LessonDto MapToDto(Lesson lesson)
    {
        return new LessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Description = lesson.Description,
            IsFree = lesson.IsFree,
            CourseId = lesson.CourseId,
            CourseTitle = lesson.Course?.Title,
            MediaFileCount = lesson.MediaFiles?.Count ?? 0,
            Order = lesson.Order,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };
    }

    #endregion
}
