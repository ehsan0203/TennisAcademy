using AutoMapper;
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

    public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all courses with optional filtering
    /// </summary>
    public async Task<PaginatedResult<CourseDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? levelId = null, int? statusId = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        var query = _unitOfWork.Repository<Course>().GetQueryable();

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
        var totalCount = await _unitOfWork.Repository<Course>().CountAsync(query);

        // Apply pagination
        var courses = await _unitOfWork.Repository<Course>().GetPagedAsync(query, page, pageSize);

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
    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
        return course != null ? _mapper.Map<CourseDto>(course) : null;
    }

    /// <summary>
    /// Get courses by level ID
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetByLevelAsync(int levelId)
    {
        var courses = await _unitOfWork.Repository<Course>().GetAllAsync(c => c.LevelId == levelId);
        return courses.Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Get courses by status ID
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetByStatusAsync(int statusId)
    {
        var courses = await _unitOfWork.Repository<Course>().GetAllAsync(c => c.StatusId == statusId);
        return courses.Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Create new course
    /// </summary>
    public async Task<CourseDto> CreateAsync(CreateCourseDto createCourseDto)
    {
        var course = _mapper.Map<Course>(createCourseDto);
        var createdCourse = await _unitOfWork.Repository<Course>().AddAsync(course);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CourseDto>(createdCourse);
    }

    /// <summary>
    /// Update existing course
    /// </summary>
    public async Task<CourseDto> UpdateAsync(int id, UpdateCourseDto updateCourseDto)
    {
        var existingCourse = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
        if (existingCourse == null)
            throw new ArgumentException($"Course with ID {id} not found");

        // Only update non-null properties
        if (updateCourseDto.Title != null)
            existingCourse.Title = updateCourseDto.Title;
        if (updateCourseDto.Description != null)
            existingCourse.Description = updateCourseDto.Description;
        if (updateCourseDto.ImageIcon != null)
            existingCourse.ImageIcon = updateCourseDto.ImageIcon;
        if (updateCourseDto.Poster != null)
            existingCourse.Poster = updateCourseDto.Poster;
        if (updateCourseDto.Price.HasValue)
            existingCourse.Price = updateCourseDto.Price.Value;
        if (updateCourseDto.LevelId.HasValue)
            existingCourse.LevelId = updateCourseDto.LevelId.Value;
        if (updateCourseDto.StatusId.HasValue)
            existingCourse.StatusId = updateCourseDto.StatusId.Value;

        existingCourse.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Course>().UpdateAsync(existingCourse);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CourseDto>(existingCourse);
    }

    /// <summary>
    /// Delete course
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
        if (course == null)
            return false;

        // Check if course has enrollments
        var hasEnrollments = await _unitOfWork.Repository<UserCourseHistory>().AnyAsync(uch => uch.CourseId == id);
        if (hasEnrollments)
        {
            // Instead of deleting, set status to archived
            course.StatusId = 4; // Assuming 4 is Archived status
            await _unitOfWork.Repository<Course>().UpdateAsync(course);
        }
        else
        {
            await _unitOfWork.Repository<Course>().DeleteAsync(id);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Change course status
    /// </summary>
    public async Task<CourseDto> ChangeStatusAsync(int id, int statusId)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
        if (course == null)
            throw new ArgumentException($"Course with ID {id} not found");

        course.StatusId = statusId;
        course.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Course>().UpdateAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CourseDto>(course);
    }

    /// <summary>
    /// Change course level
    /// </summary>
    public async Task<CourseDto> ChangeLevelAsync(int id, int levelId)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
        if (course == null)
            throw new ArgumentException($"Course with ID {id} not found");

        course.LevelId = levelId;
        course.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Course>().UpdateAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CourseDto>(course);
    }

    /// <summary>
    /// Update course price
    /// </summary>
    public async Task<CourseDto> UpdatePriceAsync(int id, decimal price)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
        if (course == null)
            throw new ArgumentException($"Course with ID {id} not found");

        course.Price = price;
        course.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Course>().UpdateAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CourseDto>(course);
    }

    /// <summary>
    /// Get popular courses (by purchase count)
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetPopularCoursesAsync(int count = 10)
    {
        var courses = await _unitOfWork.Repository<Course>().GetAllAsync();
        var popularCourses = courses
            .OrderByDescending(c => c.UserCourseHistory.Count)
            .Take(count);
        
        return popularCourses.Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Get free courses
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetFreeCoursesAsync()
    {
        var courses = await _unitOfWork.Repository<Course>().GetAllAsync(c => c.Price == 0);
        return courses.Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Get courses with advanced filtering
    /// </summary>
    public async Task<PaginatedResult<CourseDto>> GetFilteredAsync(CourseFilterDto filter)
    {
        var query = _unitOfWork.Repository<Course>().GetQueryable();

        // Status filter
        if (filter.StatusId.HasValue)
            query = query.Where(c => c.StatusId == filter.StatusId.Value);

        // Search term filter
        if (!string.IsNullOrEmpty(filter.SearchTerm))
            query = query.Where(c => c.Title.Contains(filter.SearchTerm) ||
                                     (c.Description != null && c.Description.Contains(filter.SearchTerm)));

        // Level filter
        if (filter.LevelId.HasValue)
            query = query.Where(c => c.LevelId == filter.LevelId.Value);

        // Price filters
        if (filter.MinPrice.HasValue)
            query = query.Where(c => c.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(c => c.Price <= filter.MaxPrice.Value);

        // Free courses filter (smart handling)
        if (filter.FreeOnly.HasValue && filter.FreeOnly.Value)
        {
            var anyFreeCourses = await _unitOfWork.Repository<Course>()
                .AnyAsync(c => c.Price == 0 && (!filter.StatusId.HasValue || c.StatusId == filter.StatusId.Value));

            if (anyFreeCourses)
                query = query.Where(c => c.Price == 0);
        }

        // Sorting
        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            query = filter.SortBy.ToLower() switch
            {
                "title" => filter.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
                "price" => filter.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price),
                "createdat" => filter.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                _ => query.OrderBy(c => c.CreatedAt)
            };
        }
        else
        {
            query = query.OrderBy(c => c.CreatedAt);
        }

        // Total count
        var totalCount = await _unitOfWork.Repository<Course>().CountAsync(query);

        // Pagination
        var courses = await _unitOfWork.Repository<Course>().GetPagedAsync(query, filter.Page, filter.PageSize);

        // Map to DTOs
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
    public async Task<PaginatedResult<CourseDto>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        var query = _unitOfWork.Repository<Course>().GetQueryable();
        
        query = query.Where(c => c.Title.Contains(searchTerm) || (c.Description != null && c.Description.Contains(searchTerm)));
        
        var totalCount = await _unitOfWork.Repository<Course>().CountAsync(query);
        var courses = await _unitOfWork.Repository<Course>().GetPagedAsync(query, page, pageSize);
        
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
    /// Get recommended courses for user
    /// </summary>
    public async Task<IEnumerable<CourseDto>> GetRecommendedAsync(int userId, int count = 5)
    {
        // Get user's enrolled courses to understand their level
        var userEnrollments = await _unitOfWork.Repository<UserCourseHistory>().GetAllAsync(uch => uch.AccountId == userId);
        
        if (!userEnrollments.Any())
        {
            // If no enrollments, return beginner courses
            var beginnerCourses = await _unitOfWork.Repository<Course>().GetAllAsync(c => c.LevelId == 1 && c.StatusId == 2); // Assuming 1=Beginner, 2=Active
            return beginnerCourses.Take(count).Select(c => _mapper.Map<CourseDto>(c));
        }

        // Get user's preferred level (most enrolled level)
        var userLevel = userEnrollments
            .GroupBy(uch => uch.Course.LevelId)
            .OrderByDescending(g => g.Count())
            .First().Key;

        // Get courses at the same level or one level higher
        var recommendedCourses = await _unitOfWork.Repository<Course>().GetAllAsync(c => 
            (c.LevelId == userLevel || c.LevelId == userLevel + 1) && 
            c.StatusId == 2 && // Active status
            !userEnrollments.Any(uch => uch.CourseId == c.Id)); // Not already enrolled

        return recommendedCourses.Take(count).Select(c => _mapper.Map<CourseDto>(c));
    }

    /// <summary>
    /// Get course statistics
    /// </summary>
    public async Task<CourseStatisticsDto> GetStatisticsAsync()
    {
        var allCourses = await _unitOfWork.Repository<Course>().GetAllAsync();
        var allEnrollments = await _unitOfWork.Repository<UserCourseHistory>().GetAllAsync();

        var statistics = new CourseStatisticsDto
        {
            TotalCourses = allCourses.Count(),
            ActiveCourses = allCourses.Count(c => c.StatusId == 2), 
            DraftCourses = allCourses.Count(c => c.StatusId == 1), 
            ArchivedCourses = allCourses.Count(c => c.StatusId == 4), 
            TotalEnrollments = allEnrollments.Count(),
            ActiveEnrollments = allEnrollments.Count(uch => uch.StatusId == 1), 
            CompletedCourses = allEnrollments.Count(uch => uch.StatusId == 2), 
            TotalRevenue = allEnrollments.Sum(uch => uch.Course.Price),
            CompletionRate = allEnrollments.Any() ? (double)allEnrollments.Count(uch => uch.StatusId == 2) / allEnrollments.Count() * 100 : 0
        };

        // Get most popular level
        var mostPopularLevel = allEnrollments
            .GroupBy(uch => uch.Course.LevelId)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();
        
        if (mostPopularLevel != null)
        {
            var level = await _unitOfWork.Repository<Level>().GetByIdAsync(mostPopularLevel.Key);
            statistics.MostPopularLevel = level?.Title;
        }

        // Get most enrolled course
        var mostEnrolledCourse = allEnrollments
            .GroupBy(uch => uch.CourseId)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();
        
        if (mostEnrolledCourse != null)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(mostEnrolledCourse.Key);
            statistics.MostEnrolledCourse = course?.Title;
        }

        return statistics;
    }

    /// <summary>
    /// Toggle course status (active/inactive)
    /// </summary>
    public async Task<CourseDto> ToggleStatusAsync(int id)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(id);
        if (course == null)
            throw new ArgumentException($"Course with ID {id} not found");

        // Toggle between Active (2) and Inactive (3) - adjust status IDs as needed
        course.StatusId = course.StatusId == 2 ? 3 : 2;
        course.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Course>().UpdateAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CourseDto>(course);
    }
}
