using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Application.DTOs.Course;
using MTA.Application.Services.Interface;
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
    private readonly ICurrentUser _currentUser;


    public LessonService(IUnitOfWork unitOfWork, ILogger<LessonService> logger, IMapper mapper, IMediaFileService mediaFileService, ICurrentUser currentUser)
	{
		_unitOfWork = unitOfWork;
		_logger = logger;
		_mapper = mapper;
		_mediaFileService = mediaFileService;
		_currentUser = currentUser;
	}

	/// <summary>
	/// Course IDs the caller has paid for. Empty for anonymous visitors.
	/// </summary>
	private async Task<HashSet<int>> GetPurchasedCourseIdsAsync(CancellationToken ct)
	{
		var accountId = _currentUser.Id;
		if (accountId is null) return new HashSet<int>();

		var ids = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
			.AsNoTracking()
			.Where(uch => uch.AccountId == accountId.Value)
			.Select(uch => uch.CourseId)
			.ToListAsync(ct);

		return ids.ToHashSet();
	}

	/// <summary>
	/// Get all lessons with optional filtering
	/// </summary>
	public async Task<PaginatedResult<LessonDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? courseId = null, bool? isFree = null, CancellationToken ct = default)
	{
		try
		{
			var query = _unitOfWork.Repository<Lesson>().GetQueryable()
				.AsNoTracking()
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
			var totalCount = await query.CountAsync(ct);

			// Apply pagination and ordering
			var lessons = await query
				.OrderBy(l => l.CourseId)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(ct);

			// Map to DTOs
			var purchased = await GetPurchasedCourseIdsAsync(ct);
			var lessonDtos = lessons.Select(l => MapToDto(l, purchased)).ToList();

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
	public async Task<LessonDto?> GetByIdAsync(int id, CancellationToken ct = default)
	{
		try
		{
			var lesson = await _unitOfWork.Repository<Lesson>().GetQueryable()
				.AsNoTracking()
				.Include(l => l.Course)
				.Include(l => l.MediaFile)
				.FirstOrDefaultAsync(l => l.Id == id, ct);

			if (lesson == null) return null;
			return MapToDto(lesson, await GetPurchasedCourseIdsAsync(ct));
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
	public async Task<IEnumerable<LessonDto>> GetByCourseAsync(int courseId, CancellationToken ct = default)
	{
		try
		{
			var lessons = await _unitOfWork.Repository<Lesson>().GetQueryable()
				.AsNoTracking()
				.Include(l => l.Course)
				.Include(l => l.MediaFile)
				.Where(l => l.CourseId == courseId)
				.ToListAsync(ct);

			var purchased = await GetPurchasedCourseIdsAsync(ct);
			return lessons.Select(l => MapToDto(l, purchased));
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
	public async Task<LessonDto> CreateAsync(CreateLessonDto lessonDto, CancellationToken ct = default)
	{
		try
		{
			// Validate course exists
			var course = await _unitOfWork.Repository<Course>().GetByIdAsync(lessonDto.CourseId, ct);
			if (course == null)
			{
				throw new InvalidOperationException($"Course with ID {lessonDto.CourseId} not found");
			}

            var MediaFileInfo = new MediaFileUploadDto
            {
                MediaType = "Video",
                Title = $"{lessonDto.Title} Video"
            };

            var posterMedia = await _mediaFileService.CreateAsync(lessonDto.MediaFile, MediaFileInfo);

			var lesson = new Lesson
			{
				Title = lessonDto.Title,
				Description = lessonDto.Description,
				IsFree = lessonDto.IsFree,
				CourseId = lessonDto.CourseId,
				MediaFileId = posterMedia.Id
			};

			await _unitOfWork.Repository<Lesson>().AddAsync(lesson, ct);
			await _unitOfWork.SaveChangesAsync(ct);

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
    public async Task<LessonDto?> UpdateAsync(int id, UpdateLessonDto lessonDto, CancellationToken ct = default)
    {
        try
        {
            var lesson = await _unitOfWork.Repository<Lesson>().GetByIdAsync(id, ct);
            if (lesson == null)
                return null;

            if (lessonDto.CourseId != lesson.CourseId)
            {
                var course = await _unitOfWork.Repository<Course>().GetByIdAsync(lessonDto.CourseId, ct);
                if (course == null)
                    throw new InvalidOperationException($"Course with ID {lessonDto.CourseId} not found");
            }

            if (lessonDto.NewMediaFile != null)
            {
                var mediaFileInfo = new MediaFileUploadDto
                {
                    MediaType = "Video",
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

            await _unitOfWork.Repository<Lesson>().UpdateAsync(lesson, ct);
            await _unitOfWork.SaveChangesAsync(ct);

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
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
	{
		try
		{
			var lesson = await _unitOfWork.Repository<Lesson>().GetByIdAsync(id, ct);
			if (lesson == null)
			{
				return false;
			}

			// Prevent delete if it has a media file linked
			if (lesson.MediaFileId.HasValue)
			{
				throw new InvalidOperationException($"Cannot delete lesson '{lesson.Title}' as it has an associated media file");
			}

			await _unitOfWork.Repository<Lesson>().DeleteAsync(id, ct);
			await _unitOfWork.SaveChangesAsync(ct);

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
	public async Task<LessonDto> ChangeCourseAsync(int id, int courseId, CancellationToken ct = default)
	{
		try
		{
			var lesson = await _unitOfWork.Repository<Lesson>().GetByIdAsync(id, ct);
			if (lesson == null)
			{
				throw new InvalidOperationException($"Lesson with ID {id} not found");
			}

			// Validate new course exists
			var course = await _unitOfWork.Repository<Course>().GetByIdAsync(courseId, ct);
			if (course == null)
			{
				throw new InvalidOperationException($"Course with ID {courseId} not found");
			}

			lesson.CourseId = courseId;

			await _unitOfWork.Repository<Lesson>().UpdateAsync(lesson, ct);
			await _unitOfWork.SaveChangesAsync(ct);

			return await GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Failed to retrieve updated lesson");
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
	public async Task<LessonDto> UpdateOrderAsync(int id, int order, CancellationToken ct = default)
	{
		try
		{
			var lesson = await _unitOfWork.Repository<Lesson>().GetByIdAsync(id, ct);
			if (lesson == null)
			{
				throw new InvalidOperationException($"Lesson with ID {id} not found");
			}

			await _unitOfWork.Repository<Lesson>().UpdateAsync(lesson, ct);
			await _unitOfWork.SaveChangesAsync(ct);

			return await GetByIdAsync(id, ct) ?? new LessonDto { Id = id, Title = lesson.Title, CourseId = lesson.CourseId };
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
	/// <summary>
	/// Maps a lesson, withholding the video location unless the lesson is free or the
	/// caller has bought the course. Without this the video URL of every paid lesson is
	/// handed out to anonymous callers.
	/// </summary>
	private static LessonDto MapToDto(Lesson lesson, HashSet<int> purchasedCourseIds)
	{
		var unlocked = lesson.IsFree || purchasedCourseIds.Contains(lesson.CourseId);

		return new LessonDto
		{
			Id = lesson.Id,
			Title = lesson.Title,
			Description = lesson.Description,
			IsFree = lesson.IsFree,
			CourseId = lesson.CourseId,
			MediaFileId = unlocked ? lesson.MediaFileId : null,
			MediaFileUrl = unlocked && lesson.MediaFile != null ? lesson.MediaFile.Url : null,
			IsLocked = !unlocked,
			CreatedAt = lesson.CreatedAt,
			UpdatedAt = lesson.UpdatedAt
		};
	}

	#endregion
}
