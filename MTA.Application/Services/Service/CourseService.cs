using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MTA.Application.DTOs;
using MTA.Application.DTOs.Course;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for Course operations
/// </summary>
public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediaFileService _mediaFileService;

    public CourseService(IUnitOfWork unitOfWork, IMapper mapper, IMediaFileService mediaFileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediaFileService = mediaFileService;
    }

    /// <summary>
    /// Get all courses with optional filtering
    /// </summary>
    public async Task<PaginatedResult<CourseDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? levelId = null, int? statusId = null, decimal? minPrice = null, decimal? maxPrice = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Course>().GetQueryable()
            .AsNoTracking()
            .Include(c => c.PosterMediaFile)
            .Include(c => c.IconMediaFile)
            .Include(c => c.Level)
            .Include(c => c.Status)
            .Include(c => c.Lessons)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(c => c.Title.Contains(searchTerm) || (c.Description != null && c.Description.Contains(searchTerm)));
        }

        if (levelId.HasValue)
        {
            query = query.Where(c => c.LevelId == levelId.Value);
        }

        if (statusId.HasValue)
        {
            query = query.Where(c => c.StatusId == statusId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(c => c.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(c => c.Price <= maxPrice.Value);
        }

        // Get total count
        var totalCount = await _unitOfWork.Repository<Course>().CountAsync(query, ct);

        // Apply pagination
        var courses = await _unitOfWork.Repository<Course>().GetPagedAsync(query, page, pageSize, ct);

        // Map to DTOs
        var courseDtos = courses.Select(c => _mapper.Map<CourseDto>(c)).ToList();

        return new PaginatedResult<CourseDto>
        {
            Data = courseDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    /// <summary>
    /// Get course by ID
    /// </summary>
    public async Task<CourseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Repository<Course>().GetQueryable()
            .AsNoTracking()
            .Include(c => c.PosterMediaFile)
            .Include(c => c.IconMediaFile)
            .Include(c => c.Level)
            .Include(c => c.Status)
            .Include(c=>c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        return course != null ? _mapper.Map<CourseDto>(course) : null;
    }

    /// <summary>
    /// Get courses by level ID
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetByLevelAsync(int levelId, CancellationToken ct = default)
    {
        var courses = await _unitOfWork.Repository<Course>().GetAllAsync(c => c.LevelId == levelId, ct: ct);
        return courses.Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Get courses by status ID
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetByStatusAsync(int statusId, CancellationToken ct = default)
    {
        var courses = await _unitOfWork.Repository<Course>().GetAllAsync(c => c.StatusId == statusId, ct: ct);
        return courses.Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Create new course
    /// </summary>
    public async Task<CourseDto> CreateAsync(CreateCourseDto createCourseDto, CancellationToken ct = default)
    {
        var course = _mapper.Map<Course>(createCourseDto);

        // Poster
        if (createCourseDto.PosterFile != null)
        {
            var posterDto = new MediaFileUploadDto
            {
                MediaType = "Course",
                PlacementName = "Poster",
                Title = $"{createCourseDto.Title} Poster"
            };

            var posterMedia = await _mediaFileService.CreateAsync(createCourseDto.PosterFile, posterDto);
            course.PosterMediaFileId = posterMedia.Id;
        }

        // Icon
        if (createCourseDto.IconFile != null)
        {
            var iconDto = new MediaFileUploadDto
            {
                MediaType = "Course",
                PlacementName = "Icon",
                Title = $"{createCourseDto.Title} Icon"
            };

            var iconMedia = await _mediaFileService.CreateAsync(createCourseDto.IconFile, iconDto);
            course.IconMediaFileId = iconMedia.Id;
        }

        var createdCourse = await _unitOfWork.Repository<Course>().AddAsync(course, ct);

        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<CourseDto>(createdCourse);
    }

    /// <summary>
    /// Update existing course with media references
    /// </summary>
    public async Task<CourseDto> UpdateAsync(int id, UpdateCourseDto updateCourseDto, CancellationToken ct = default)
    {
        var existingCourse = await _unitOfWork.Repository<Course>()
            .GetQueryable()
            .Include(c => c.IconMediaFile)
            .Include(c => c.PosterMediaFile)
            .Include(c => c.Level)
            .Include(c => c.Status)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (existingCourse == null)
            throw new ArgumentException($"Course with ID {id} not found");

        // -------------------------------
        // به‌روزرسانی پراپرتی‌های ساده
        // -------------------------------
        if (!string.IsNullOrEmpty(updateCourseDto.Title))
            existingCourse.Title = updateCourseDto.Title;

        if (!string.IsNullOrEmpty(updateCourseDto.Description))
            existingCourse.Description = updateCourseDto.Description;

        if (updateCourseDto.Price.HasValue)
            existingCourse.Price = updateCourseDto.Price.Value;

        if (updateCourseDto.LevelId.HasValue)
            existingCourse.LevelId = updateCourseDto.LevelId.Value;

        if (updateCourseDto.StatusId.HasValue)
            existingCourse.StatusId = updateCourseDto.StatusId.Value;

        // -------------------------------
        // آپدیت یا ایجاد Poster
        // -------------------------------
        if (updateCourseDto.NewPoster != null)
        {
            var posterDto = new MediaFileUploadDto
            {
                MediaType = "Course",
                PlacementName = "Poster",
                Title = $"{updateCourseDto.Title ?? existingCourse.Title} Poster"
            };

            MediaFileDto posterMedia;
            if (existingCourse.PosterMediaFileId == null || existingCourse.PosterMediaFileId == 0)
            {
                posterMedia = await _mediaFileService.CreateAsync(updateCourseDto.NewPoster, posterDto);
            }
            else
            {
                posterMedia = await _mediaFileService.UpdateAsync(existingCourse.PosterMediaFileId.Value, updateCourseDto.NewPoster, posterDto);
            }

            existingCourse.PosterMediaFileId = posterMedia.Id;
        }

        // -------------------------------
        // آپدیت یا ایجاد Icon
        // -------------------------------
        if (updateCourseDto.NewIcon != null)
        {
            var iconDto = new MediaFileUploadDto
            {
                MediaType = "Course",
                PlacementName = "Icon",
                Title = $"{updateCourseDto.Title ?? existingCourse.Title} Icon"
            };

            MediaFileDto iconMedia;
            if (existingCourse.IconMediaFileId == null || existingCourse.IconMediaFileId == 0)
            {
                iconMedia = await _mediaFileService.CreateAsync(updateCourseDto.NewIcon , iconDto);
            }
            else
            {
                iconMedia = await _mediaFileService.UpdateAsync(existingCourse.IconMediaFileId.Value, updateCourseDto.NewIcon, iconDto);
            }

            existingCourse.IconMediaFileId = iconMedia.Id;
        }

            // -------------------------------
            // ذخیره تغییرات
            // -------------------------------
            await _unitOfWork.Repository<Course>().UpdateAsync(existingCourse, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return _mapper.Map<CourseDto>(existingCourse);
        }

    /// <summary>
    /// Delete course or archive if it has enrollments
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Repository<Course>()
            .GetQueryable()
            .Include(c => c.UserCourseHistory)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (course == null)
            return false;

        // Check if course has enrollments
        if (course.UserCourseHistory.Any())
        {
            // Archive instead of delete
            course.StatusId = 4; // Archived
            await _unitOfWork.Repository<Course>().UpdateAsync(course, ct);
        }
        else
        {
            await _unitOfWork.Repository<Course>().DeleteAsync(id, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }


    /// <summary>
    /// Change course status
    /// </summary>
    public async Task<CourseDto> ChangeStatusAsync(int id, int statusId, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id, ct);
        if (course == null)
            throw new ArgumentException($"Course with ID {id} not found");

        course.StatusId = statusId;

        await _unitOfWork.Repository<Course>().UpdateAsync(course, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<CourseDto>(course);
    }

    /// <summary>
    /// Change course level
    /// </summary>
    public async Task<CourseDto> ChangeLevelAsync(int id, int levelId, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id, ct);
        if (course == null)
            throw new ArgumentException($"Course with ID {id} not found");

        course.LevelId = levelId;

        await _unitOfWork.Repository<Course>().UpdateAsync(course, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<CourseDto>(course);
    }

    /// <summary>
    /// Update course price
    /// </summary>
    public async Task<CourseDto> UpdatePriceAsync(int id, decimal price, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id, ct);
        if (course == null)
            throw new ArgumentException($"Course with ID {id} not found");

        course.Price = price;

        await _unitOfWork.Repository<Course>().UpdateAsync(course, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<CourseDto>(course);
    }

    /// <summary>
    /// Get popular courses ordered by enrollment count, resolved in the database.
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetPopularCoursesAsync(int count = 10, CancellationToken ct = default)
    {
        var courses = await _unitOfWork.Repository<Course>().GetQueryable()
            .AsNoTracking()
            .Include(c => c.Level)
            .Include(c => c.Status)
            .Include(c => c.IconMediaFile)
            .Include(c => c.PosterMediaFile)
            .GroupJoin(
                _unitOfWork.Repository<UserCourseHistory>().GetQueryable().AsNoTracking(),
                c => c.Id,
                uch => uch.CourseId,
                (c, enrollments) => new { Course = c, EnrollmentCount = enrollments.Count() })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(count)
            .Select(x => x.Course)
            .ToListAsync(ct);

        return courses.Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Get free courses
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetFreeCoursesAsync(CancellationToken ct = default)
    {
        var courses = await _unitOfWork.Repository<Course>().GetAllAsync(c => c.Price == 0, ct: ct);
        return courses.Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Get courses with advanced filtering
    /// </summary>
    public async Task<PaginatedResult<CourseDto>> GetFilteredAsync(CourseFilterDto filter, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Course>().GetQueryable()
            .AsNoTracking()
            .Include(c => c.IconMediaFile)
            .Include(c => c.PosterMediaFile)
            .Include(c => c.Level)
            .Include(c => c.Status)
            .AsQueryable();


        // Search term filter
        if (!string.IsNullOrEmpty(filter.SearchTerm))
            query = query.Where(c => c.Title.Contains(filter.SearchTerm) ||
                                     (c.Description != null && c.Description.Contains(filter.SearchTerm)));

        if (filter.LevelId.HasValue && filter.LevelId.Value > 0)
            query = query.Where(c => c.LevelId == filter.LevelId.Value);

        if (filter.StatusId.HasValue && filter.StatusId.Value > 0)
            query = query.Where(c => c.StatusId == filter.StatusId.Value);

        if (filter.MinPrice.HasValue && filter.MinPrice.Value > 0)
            query = query.Where(c => c.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue && filter.MaxPrice.Value > 0)
            query = query.Where(c => c.Price <= filter.MaxPrice.Value);

        // Free courses filter
        if (filter.FreeOnly.HasValue && filter.FreeOnly.Value)
            query = query.Where(c => c.Price == 0);

        // Sorting
        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
            "price" => filter.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price),
            "createdat" => filter.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
            _ => query.OrderBy(c => c.CreatedAt)
        };

        var totalCount = await _unitOfWork.Repository<Course>().CountAsync(query, ct);
        var courses = await _unitOfWork.Repository<Course>().GetPagedAsync(query, filter.Page, filter.PageSize, ct);

        var courseDtos = courses.Select(c => _mapper.Map<CourseDto>(c)).ToList();

        return new PaginatedResult<CourseDto>
        {
            Data = courseDtos,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize,
        };
    }

    /// <summary>
    /// Search courses by text
    /// </summary>
    public async Task<PaginatedResult<CourseDto>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new PaginatedResult<CourseDto>
            {
                Data = new List<CourseDto>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };

        var query = _unitOfWork.Repository<Course>().GetQueryable()
            .AsNoTracking()
            .Include(c => c.IconMediaFile)
            .Include(c => c.PosterMediaFile)
            .Include(c => c.Level)
            .Include(c => c.Status)
            .Where(c => c.Title.Contains(searchTerm) || (c.Description != null && c.Description.Contains(searchTerm)));

        var totalCount = await _unitOfWork.Repository<Course>().CountAsync(query, ct);
        var courses = await _unitOfWork.Repository<Course>().GetPagedAsync(query, page, pageSize, ct);

        var courseDtos = courses.Select(c => _mapper.Map<CourseDto>(c)).ToList();

        return new PaginatedResult<CourseDto>
        {
            Data = courseDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Get recommended courses for user
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetRecommendedAsync(int userId, int count = 5, CancellationToken ct = default)
    {
        // Get user's enrolled courses with course info
        var userEnrollments = await _unitOfWork.Repository<UserCourseHistory>()
            .GetQueryable()
            .AsNoTracking()
            .Include(uch => uch.Course)
                .ThenInclude(c => c.Level)
            .Include(uch => uch.Course)
                .ThenInclude(c => c.Status)
            .Where(uch => uch.AccountId == userId)
            .ToListAsync(ct);

        if (!userEnrollments.Any())
        {
            // No enrollments, return beginner courses
            var beginnerCourses = await _unitOfWork.Repository<Course>()
                .GetQueryable()
                .AsNoTracking()
                .Include(c => c.IconMediaFile)
                .Include(c => c.PosterMediaFile)
                .Include(c => c.Level)
                .Include(c => c.Status)
                .Where(c => c.LevelId == 1 && c.StatusId == 2)
                .ToListAsync(ct);

            return beginnerCourses.Take(count).Select(c => _mapper.Map<CourseDto>(c));
        }

        // User's most enrolled level
        var userLevel = userEnrollments
            .GroupBy(uch => uch.Course.LevelId)
            .OrderByDescending(g => g.Count())
            .First().Key;

        // Recommended courses: same level or one level higher, not enrolled yet
        var recommendedCourses = await _unitOfWork.Repository<Course>()
            .GetQueryable()
            .AsNoTracking()
            .Include(c => c.IconMediaFile)
            .Include(c => c.PosterMediaFile)
            .Include(c => c.Level)
            .Include(c => c.Status)
            .Where(c => (c.LevelId == userLevel || c.LevelId == userLevel + 1)
                        && c.StatusId == 2
                        && !userEnrollments.Any(uch => uch.CourseId == c.Id))
            .ToListAsync(ct);

        return recommendedCourses.Take(count).Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Get course statistics
    /// </summary>
    public async Task<CourseStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        var courses = _unitOfWork.Repository<Course>().GetQueryable().AsNoTracking();
        var enrollments = _unitOfWork.Repository<UserCourseHistory>().GetQueryable().AsNoTracking();

        var totalEnrollments = await enrollments.CountAsync(ct);
        var completedCount = await enrollments.CountAsync(uch => uch.StatusId == 2, ct);

        var statistics = new CourseStatisticsDto
        {
            TotalCourses = await courses.CountAsync(ct),
            ActiveCourses = await courses.CountAsync(c => c.StatusId == 2, ct),
            DraftCourses = await courses.CountAsync(c => c.StatusId == 1, ct),
            ArchivedCourses = await courses.CountAsync(c => c.StatusId == 4, ct),
            TotalEnrollments = totalEnrollments,
            ActiveEnrollments = await enrollments.CountAsync(uch => uch.StatusId == 1, ct),
            CompletedCourses = completedCount,
            TotalRevenue = await enrollments.SumAsync(uch => uch.Course.Price, ct),
            CompletionRate = totalEnrollments > 0
                ? (double)completedCount / totalEnrollments * 100
                : 0
        };

        // Most popular level (DB-side group by, take top)
        var mostPopularLevelId = await enrollments
            .GroupBy(uch => uch.Course.LevelId)
            .OrderByDescending(g => g.Count())
            .Select(g => (int?)g.Key)
            .FirstOrDefaultAsync(ct);

        if (mostPopularLevelId.HasValue)
        {
            var level = await _unitOfWork.Repository<Level>().GetByIdAsync(mostPopularLevelId.Value, ct);
            statistics.MostPopularLevel = level?.Title;
        }

        // Most enrolled course (DB-side group by, take top)
        var mostEnrolledCourseId = await enrollments
            .GroupBy(uch => uch.CourseId)
            .OrderByDescending(g => g.Count())
            .Select(g => (int?)g.Key)
            .FirstOrDefaultAsync(ct);

        if (mostEnrolledCourseId.HasValue)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(mostEnrolledCourseId.Value, ct);
            statistics.MostEnrolledCourse = course?.Title;
        }

        return statistics;
    }

    /// <summary>
    /// Toggle course status (active/inactive)
    /// </summary>
    public async Task<CourseDto> ToggleStatusAsync(int id, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Repository<Course>()
            .GetQueryable()
            .Include(c => c.Level)
            .Include(c => c.Status)
            .Include(c => c.IconMediaFile)
            .Include(c => c.PosterMediaFile)
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(ct);

        if (course == null)
            throw new ArgumentException($"Course with ID {id} not found");

        // Toggle Active (2) / Inactive (3)
        course.StatusId = course.StatusId == 2 ? 3 : 2;

        await _unitOfWork.Repository<Course>().UpdateAsync(course, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<CourseDto>(course);
    }



}
