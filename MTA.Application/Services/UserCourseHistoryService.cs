using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for UserCourseHistory operations
/// </summary>
public class UserCourseHistoryService : IUserCourseHistoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserCourseHistoryService> _logger;

    public UserCourseHistoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserCourseHistoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<UserCourseHistoryDto>> GetAllAsync(int page = 1, int pageSize = 10, int? accountId = null, int? courseId = null)
    {
        try
        {
            var query = _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .Include(uch => uch.Course)
                .Include(uch => uch.Account)
                    .ThenInclude(acc => acc.UserProfile)
                .Include(uch => uch.Status)
                .AsQueryable();

            // Apply filters
            if (accountId.HasValue)
                query = query.Where(uch => uch.AccountId == accountId.Value);

            if (courseId.HasValue)
                query = query.Where(uch => uch.CourseId == courseId.Value);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var items = await query
                .OrderByDescending(uch => uch.EnrolledAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<UserCourseHistoryDto>>(items);

            return new PaginatedResult<UserCourseHistoryDto>
            {
                Data = dtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all user course histories");
            throw;
        }
    }

    public async Task<UserCourseHistoryDto?> GetByIdAsync(int id)
    {
        try
        {
            var userCourseHistory = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .Include(uch => uch.Course)
                .Include(uch => uch.Account)
                    .ThenInclude(acc => acc.UserProfile)
                .Include(uch => uch.Status)
                .FirstOrDefaultAsync(uch => uch.Id == id);

            return userCourseHistory != null ? _mapper.Map<UserCourseHistoryDto>(userCourseHistory) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user course history by ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<UserCourseHistoryDto>> GetByAccountAsync(int accountId)
    {
        try
        {
            var userCourseHistories = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .Include(uch => uch.Course)
                .Include(uch => uch.Account)
                    .ThenInclude(acc => acc.UserProfile)
                .Include(uch => uch.Status)
                .Where(uch => uch.AccountId == accountId)
                .OrderByDescending(uch => uch.EnrolledAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserCourseHistoryDto>>(userCourseHistories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user course histories by account ID: {AccountId}", accountId);
            throw;
        }
    }

    public async Task<IEnumerable<UserCourseHistoryDto>> GetByCourseAsync(int courseId)
    {
        try
        {
            var userCourseHistories = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .Include(uch => uch.Course)
                .Include(uch => uch.Account)
                    .ThenInclude(acc => acc.UserProfile)
                .Include(uch => uch.Status)
                .Where(uch => uch.CourseId == courseId)
                .OrderByDescending(uch => uch.EnrolledAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserCourseHistoryDto>>(userCourseHistories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user course histories by course ID: {CourseId}", courseId);
            throw;
        }
    }

    public async Task<bool> UserHasPurchasedCourseAsync(int accountId, int courseId)
    {
        try
        {
            return await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .AnyAsync(uch => uch.AccountId == accountId && uch.CourseId == courseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user has purchased course: AccountId={AccountId}, CourseId={CourseId}", accountId, courseId);
            throw;
        }
    }

    public async Task<UserCourseHistoryDto> CreateAsync(UserCourseHistoryDto userCourseHistoryDto)
    {
        try
        {
            // Check if user already has this course
            var existing = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .AnyAsync(uch => uch.AccountId == userCourseHistoryDto.AccountId && uch.CourseId == userCourseHistoryDto.CourseId);

            if (existing)
                throw new InvalidOperationException("User has already purchased this course");

            var userCourseHistory = _mapper.Map<UserCourseHistory>(userCourseHistoryDto);
            userCourseHistory.EnrolledAt = DateTime.UtcNow;
            userCourseHistory.StatusId = 1; // Assuming 1 is "Active" status

            var created = await _unitOfWork.Repository<UserCourseHistory>().AddAsync(userCourseHistory);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(created.Id) ?? throw new InvalidOperationException("Failed to retrieve created user course history");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user course history");
            throw;
        }
    }

    public async Task<UserCourseHistoryDto> UpdateAsync(int id, UserCourseHistoryDto userCourseHistoryDto)
    {
        try
        {
            var existing = await _unitOfWork.Repository<UserCourseHistory>().GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException($"User course history with ID {id} not found");

            // Update properties
            existing.CourseId = userCourseHistoryDto.CourseId;
            existing.AccountId = userCourseHistoryDto.AccountId;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _unitOfWork.Repository<UserCourseHistory>().UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(updated.Id) ?? throw new InvalidOperationException("Failed to retrieve updated user course history");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user course history with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var deleted = await _unitOfWork.Repository<UserCourseHistory>().DeleteAsync(id);
            if (deleted)
                await _unitOfWork.SaveChangesAsync();

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user course history with ID: {Id}", id);
            throw;
        }
    }

    public async Task<UserCourseHistoryStatisticsDto> GetStatisticsAsync()
    {
        try
        {
            var allUserCourseHistories = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .Include(uch => uch.Course)
                .ToListAsync();

            var totalPurchases = allUserCourseHistories.Count;
            var totalRevenue = allUserCourseHistories.Sum(uch => uch.Course?.Price ?? 0);
            var uniqueUsers = allUserCourseHistories.Select(uch => uch.AccountId).Distinct().Count();
            var uniqueCourses = allUserCourseHistories.Select(uch => uch.CourseId).Distinct().Count();

            var averageCoursesPerUser = uniqueUsers > 0 ? (double)totalPurchases / uniqueUsers : 0;
            var averageRevenuePerUser = uniqueUsers > 0 ? totalRevenue / uniqueUsers : 0;

            var thisMonth = DateTime.UtcNow.Month;
            var lastMonth = DateTime.UtcNow.AddMonths(-1).Month;

            var purchasesThisMonth = allUserCourseHistories.Count(uch => uch.EnrolledAt.Month == thisMonth);
            var purchasesLastMonth = allUserCourseHistories.Count(uch => uch.EnrolledAt.Month == lastMonth);
            var revenueThisMonth = allUserCourseHistories.Where(uch => uch.EnrolledAt.Month == thisMonth).Sum(uch => uch.Course?.Price ?? 0);
            var revenueLastMonth = allUserCourseHistories.Where(uch => uch.EnrolledAt.Month == lastMonth).Sum(uch => uch.Course?.Price ?? 0);

            return new UserCourseHistoryStatisticsDto
            {
                TotalPurchases = totalPurchases,
                TotalRevenue = totalRevenue,
                UniqueUsers = uniqueUsers,
                UniqueCourses = uniqueCourses,
                AverageCoursesPerUser = averageCoursesPerUser,
                AverageRevenuePerUser = (double)averageRevenuePerUser,
                PurchasesThisMonth = purchasesThisMonth,
                PurchasesLastMonth = purchasesLastMonth,
                RevenueThisMonth = revenueThisMonth,
                RevenueLastMonth = revenueLastMonth
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user course history statistics");
            throw;
        }
    }

    public async Task<IEnumerable<UserCourseHistoryDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var userCourseHistories = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .Include(uch => uch.Course)
                .Include(uch => uch.Account)
                    .ThenInclude(acc => acc.UserProfile)
                .Include(uch => uch.Status)
                .Where(uch => uch.EnrolledAt >= startDate && uch.EnrolledAt <= endDate)
                .OrderByDescending(uch => uch.EnrolledAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserCourseHistoryDto>>(userCourseHistories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user course histories by date range: {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    //public async Task<IEnumerable<CourseDto>> GetPopularCoursesAsync(int count = 10)
    //{
    //    try
    //    {
    //        var popularCourses = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
    //            .GroupBy(uch => uch.CourseId)
    //            .Select(g => new
    //            {
    //                CourseId = g.Key,
    //                PurchaseCount = g.Count()
    //            })
    //            .OrderByDescending(x => x.PurchaseCount)
    //            .Take(count)
    //            .ToListAsync();

    //        var courseIds = popularCourses.Select(pc => pc.CourseId).ToList();
    //        var courses = await _unitOfWork.Repository<Course>().GetQueryable()
    //            .Where(c => courseIds.Contains(c.Id))
    //            .ToListAsync();

    //        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error getting popular courses");
    //        throw;
    //    }
    //}

    //public async Task<UserLearningProgressDto> GetUserLearningProgressAsync(int accountId)
    //{
    //    try
    //    {
    //        var userCourseHistories = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
    //            .Include(uch => uch.Course)
    //                .ThenInclude(c => c.Lessons)
    //            .Include(uch => uch.Account)
    //                .ThenInclude(acc => acc.UserProfile)
    //            .Where(uch => uch.AccountId == accountId)
    //            .ToListAsync();

    //        if (!userCourseHistories.Any())
    //            throw new InvalidOperationException($"No course history found for account ID {accountId}");

    //        var firstHistory = userCourseHistories.First();
    //        var userProfile = firstHistory.Account.UserProfile;

    //        var totalCoursesPurchased = userCourseHistories.Count;
    //        var totalSpent = userCourseHistories.Sum(uch => uch.Course?.Price ?? 0);

    //        // Placeholder values for completed courses and lessons
    //        var totalCoursesCompleted = 0; // Would need additional tracking
    //        var totalLessonsCompleted = 0; // Would need additional tracking
    //        var completionRate = 0.0; // Would need additional tracking

    //        var courseProgress = userCourseHistories.Select(uch => new CourseProgressDto
    //        {
    //            CourseId = uch.CourseId,
    //            CourseTitle = uch.Course?.Title ?? "",
    //            TotalLessons = uch.Course?.Lessons?.Count ?? 0,
    //            CompletedLessons = 0, // Would need additional tracking
    //            ProgressPercentage = 0.0, // Would need additional tracking
    //            PurchaseDate = uch.EnrolledAt,
    //            LastAccessDate = uch.UpdatedAt
    //        }).ToList();

    //        return new UserLearningProgressDto
    //        {
    //            AccountId = accountId,
    //            UserFirstName = userProfile?.FirstName ?? "",
    //            UserLastName = userProfile?.LastName ?? "",
    //            TotalCoursesPurchased = totalCoursesPurchased,
    //            TotalCoursesCompleted = totalCoursesCompleted,
    //            TotalLessonsCompleted = totalLessonsCompleted,
    //            TotalSpent = totalSpent,
    //            CompletionRate = completionRate,
    //            LastActivityDate = userCourseHistories.Max(uch => uch.UpdatedAt ?? uch.CreatedAt),
    //            CourseProgress = courseProgress
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error getting user learning progress for account ID: {AccountId}", accountId);
    //        throw;
    //    }
    //}
}
