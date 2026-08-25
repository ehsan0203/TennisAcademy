using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MTA.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for UserCourseHistory operations
/// </summary>
public class UserCourseHistoryService : IUserCourseHistoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserCourseHistoryService> _logger;
    private readonly ILookupService _lookupService;

    public UserCourseHistoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserCourseHistoryService> logger, ILookupService lookupService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _lookupService = lookupService;
    }

    public async Task<PaginatedResult<UserCourseHistoryDetailDto>> GetAllAsync(int page = 1, int pageSize = 10, int? accountId = null, int? courseId = null, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .Include(uch => uch.Course)
                .Include(uch => uch.Account)
                    .ThenInclude(acc => acc.UserProfile)
                .Include(uch => uch.Status)
                .AsNoTracking();

            // Apply filters
            if (accountId.HasValue)
                query = query.Where(uch => uch.AccountId == accountId.Value);

            if (courseId.HasValue)
                query = query.Where(uch => uch.CourseId == courseId.Value);

            var totalCount = await query.CountAsync(ct);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var items = await query
                .OrderByDescending(uch => uch.EnrolledAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            var dtos = _mapper.Map<IEnumerable<UserCourseHistoryDetailDto>>(items);

            return new PaginatedResult<UserCourseHistoryDetailDto>
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

    public async Task<UserCourseHistoryDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var userCourseHistory = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .Include(uch => uch.Course)
                .Include(uch => uch.Account)
                    .ThenInclude(acc => acc.UserProfile)
                .Include(uch => uch.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(uch => uch.Id == id, ct);

            return userCourseHistory != null ? _mapper.Map<UserCourseHistoryDetailDto>(userCourseHistory) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user course history by ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<UserCourseHistoryDetailDto>> GetByAccountAsync(int accountId, CancellationToken ct = default)
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
                .AsNoTracking()
                .ToListAsync(ct);

            return _mapper.Map<IEnumerable<UserCourseHistoryDetailDto>>(userCourseHistories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user course histories by account ID: {AccountId}", accountId);
            throw;
        }
    }

    public async Task<IEnumerable<UserCourseHistoryDetailDto>> GetByCourseAsync(int courseId, CancellationToken ct = default)
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
                .AsNoTracking()
                .ToListAsync(ct);

            return _mapper.Map<IEnumerable<UserCourseHistoryDetailDto>>(userCourseHistories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user course histories by course ID: {CourseId}", courseId);
            throw;
        }
    }

    public async Task<bool> UserHasPurchasedCourseAsync(int accountId, int courseId, CancellationToken ct = default)
    {
        try
        {
            return await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .AnyAsync(uch => uch.AccountId == accountId && uch.CourseId == courseId, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user has purchased course: AccountId={AccountId}, CourseId={CourseId}", accountId, courseId);
            throw;
        }
    }

    public async Task<UpdateUserCourseHistoryDto> CreateAsync(CreateUserCourseHistoryDto userCourseHistoryDto, CancellationToken ct = default)
    {
        try
        {
            // بررسی اینکه کاربر قبلاً این دوره را خریده است
            var existing = await _unitOfWork.Repository<UserCourseHistory>()
                .GetQueryable()
                .AnyAsync(uch => uch.AccountId == userCourseHistoryDto.AccountId && uch.CourseId == userCourseHistoryDto.CourseId, ct);

            if (existing)
                throw new InvalidOperationException("User has already purchased this course");

            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(userCourseHistoryDto.CourseId, ct)
                ?? throw new KeyNotFoundException("Course not found");

            // دریافت StatusId از جدول Lookups
            var activeStatusLookup = await _lookupService.GetByCategoryAndKeyAsync("UserCourseStatus", "Active");
            if (activeStatusLookup == null)
                throw new InvalidOperationException("Active status not found in Lookups");

            var userCourseHistory = new UserCourseHistory
            {
                AccountId = userCourseHistoryDto.AccountId,
                CourseId = userCourseHistoryDto.CourseId,
                EnrolledAt = DateTime.UtcNow,
                StatusId = activeStatusLookup.Id, // مقدار پویا
                PurchasePrice = course.Price
            };

            var created = await _unitOfWork.Repository<UserCourseHistory>().AddAsync(userCourseHistory, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return _mapper.Map<UpdateUserCourseHistoryDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user course history");
            throw;
        }
    }


    public async Task<UpdateUserCourseHistoryDto> UpdateAsync(int id, UpdateUserCourseHistoryDto userCourseHistoryDto, CancellationToken ct = default)
    {
        try
        {
            var existing = await _unitOfWork.Repository<UserCourseHistory>().GetByIdAsync(id, ct);
            if (existing == null)
                throw new ArgumentException($"User course history with ID {id} not found");

            // Update properties
            existing.CourseId = userCourseHistoryDto.CourseId;
            existing.AccountId = userCourseHistoryDto.AccountId;

            var updated = await _unitOfWork.Repository<UserCourseHistory>().UpdateAsync(existing, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return _mapper.Map<UpdateUserCourseHistoryDto>(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user course history with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await _unitOfWork.Repository<UserCourseHistory>().DeleteAsync(id, ct);
            if (deleted)
                await _unitOfWork.SaveChangesAsync(ct);

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user course history with ID: {Id}", id);
            throw;
        }
    }

    public async Task<UserCourseHistoryStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        try
        {
            var allUserCourseHistories = await _unitOfWork.Repository<UserCourseHistory>().GetQueryable()
                .Include(uch => uch.Course)
                .AsNoTracking()
                .ToListAsync(ct);

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

    public async Task<IEnumerable<UserCourseHistoryDetailDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
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
                .AsNoTracking()
                .ToListAsync(ct);

            return _mapper.Map<IEnumerable<UserCourseHistoryDetailDto>>(userCourseHistories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user course histories by date range: {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }
}
