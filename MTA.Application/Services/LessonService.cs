using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Application.DTOs.Course;
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
    private readonly IMediaFileService _mediaFileService;


    public LessonService(IUnitOfWork unitOfWork, ILogger<LessonService> logger, IMapper mapper, IMediaFileService mediaFileService)
	{
		_unitOfWork = unitOfWork;
		_logger = logger;
		_mapper = mapper;
		_mediaFileService = mediaFileService;
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
				.Include(l => l.MediaFile)
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
				.Include(l => l.MediaFile)
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
				.Include(l => l.MediaFile)
				.Where(l => l.CourseId == courseId)
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

            var MediaFileInfo = new MediaFileUploadDto
            {
                MediaType = "Video",
                PlacementName = "None",
                Title = $"{lessonDto.Title} Video"
            };

            var posterMedia = await _mediaFileService.CreateAsync(lessonDto.MediaFile, MediaFileInfo);

			var lesson = new Lesson
			{
				Title = lessonDto.Title,
				Description = lessonDto.Description,
				IsFree = lessonDto.IsFree,
				CourseId = lessonDto.CourseId,
				MediaFileId = posterMedia.Id,
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
    /// <summary>
    /// Update existing lesson
    /// </summary>
    public async Task<LessonDto> UpdateAsync(int id, UpdateLessonDto lessonDto)
    {
        try
        {
            var lesson = await _unitOfWork.Repository<Lesson>().GetByIdAsync(id);
            if (lesson == null)
                throw new InvalidOperationException($"Lesson with ID {id} not found");

            if (lessonDto.CourseId != lesson.CourseId)
            {
                var course = await _unitOfWork.Repository<Course>().GetByIdAsync(lessonDto.CourseId);
                if (course == null)
                    throw new InvalidOperationException($"Course with ID {lessonDto.CourseId} not found");
            }

            if (lessonDto.NewMediaFile != null)
            {
                var mediaFileInfo = new MediaFileUploadDto
                {
                    MediaType = "Video",
                    PlacementName = "None",
                    Title = $"{lessonDto.Title ?? lesson.Title} Video"
                };

                MediaFileDto posterMedia;

                if (lesson.MediaFileId == 0 || lesson.MediaFileId == null)
                {
                    posterMedia = await _mediaFileService.CreateAsync(lessonDto.NewMediaFile, mediaFileInfo);
                }
                else
                {
                    posterMedia = await _mediaFileService.UpdateAsync(
                        lesson.MediaFileId!.Value,
                        lessonDto.NewMediaFile,
                        mediaFileInfo
                    );
                }

                lesson.MediaFileId = posterMedia.Id;
            }
            else if (lessonDto.MediaFileId.HasValue)
            {
                lesson.MediaFileId = lessonDto.MediaFileId.Value;
            }

            if (!string.IsNullOrEmpty(lessonDto.Title))
                lesson.Title = lessonDto.Title;

            if (!string.IsNullOrEmpty(lessonDto.Description))
                lesson.Description = lessonDto.Description;

            lesson.IsFree = lessonDto.IsFree ?? true;

            if (lessonDto.CourseId != 0)
                lesson.CourseId = lessonDto.CourseId;

            lesson.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Lesson>().UpdateAsync(lesson);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LessonDto>(lesson);
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

			// Prevent delete if it has a media file linked
			if (lesson.MediaFileId.HasValue)
			{
				throw new InvalidOperationException($"Cannot delete lesson '{lesson.Title}' as it has an associated media file");
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

			lesson.UpdatedAt = DateTime.UtcNow;

			_unitOfWork.Repository<Lesson>().UpdateAsync(lesson);
			await _unitOfWork.SaveChangesAsync();

			return await GetByIdAsync(id) ?? new LessonDto { Id = id, Title = lesson.Title, CourseId = lesson.CourseId };
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
			MediaFileId = lesson.MediaFileId,
			MediaFileUrl = lesson.MediaFile != null ? lesson.MediaFile.Url : null,
			CreatedAt = lesson.CreatedAt,
			UpdatedAt = lesson.UpdatedAt
		};
	}

	#endregion
}
