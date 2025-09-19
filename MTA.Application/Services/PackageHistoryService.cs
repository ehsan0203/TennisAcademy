using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for PackageHistory operations
/// </summary>
public class PackageHistoryService : IPackageHistoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PackageHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all package histories with optional filtering
    /// </summary>
    public async Task<PaginatedResult<PackageHistoryDto>> GetAllAsync(int page = 1, int pageSize = 10, int? accountId = null, int? packageId = null, bool? isExpired = null)
    {
        var query = _unitOfWork.Repository<PackageHistory>().GetQueryable();

        // Apply filters
        if (accountId.HasValue)
        {
            query = query.Where(ph => ph.AccountId == accountId.Value);
        }

        if (packageId.HasValue)
        {
            query = query.Where(ph => ph.PackageId == packageId.Value);
        }

        if (isExpired.HasValue)
        {
            var currentDate = DateTime.UtcNow;
            if (isExpired.Value)
            {
                query = query.Where(ph => ph.ExpiredDate < currentDate);
            }
            else
            {
                query = query.Where(ph => ph.ExpiredDate >= currentDate);
            }
        }

        // Get total count
        var totalCount = await _unitOfWork.Repository<PackageHistory>().CountAsync(query);

        // Apply pagination
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetPagedAsync(query, page, pageSize);

        // Map to DTOs with additional data
        var packageHistoryDtos = packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph)).ToList();

        return new PaginatedResult<PackageHistoryDto>
        {
            Data = packageHistoryDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    /// <summary>
    /// Get package history by ID
    /// </summary>
    public async Task<PackageHistoryDto?> GetByIdAsync(int id)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id);
        return packageHistory != null ? _mapper.Map<PackageHistoryDto>(packageHistory) : null;
    }

    /// <summary>
    /// Get package histories by account ID
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetByAccountAsync(int accountId)
    {
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => ph.AccountId == accountId);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get package histories by package ID
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetByPackageAsync(int packageId)
    {
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => ph.PackageId == packageId);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get active package histories for user
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetActiveByAccountAsync(int accountId)
    {
        var currentDate = DateTime.UtcNow;
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => 
            ph.AccountId == accountId && ph.ExpiredDate >= currentDate);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get expired package histories for user
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetExpiredByAccountAsync(int accountId)
    {
        var currentDate = DateTime.UtcNow;
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => 
            ph.AccountId == accountId && ph.ExpiredDate < currentDate);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Check if user has active package
    /// </summary>
    public async Task<bool> UserHasActivePackageAsync(int accountId, int packageId)
    {
        var currentDate = DateTime.UtcNow;
        var activePackage = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => 
            ph.AccountId == accountId && 
            ph.PackageId == packageId && 
            ph.ExpiredDate >= currentDate);
        return activePackage.Any();
    }

    /// <summary>
    /// Create new package history
    /// </summary>
    //public async Task<PackageHistoryDto> CreateAsync(CreatePackageHistoryDto packageHistoryDto)
    //{
    //    var packageHistory = _mapper.Map<PackageHistory>(packageHistoryDto);
    //    packageHistory.CreatedAt = DateTime.UtcNow;

    //    var createdPackageHistory = await _unitOfWork.Repository<PackageHistory>().AddAsync(packageHistory);
    //    await _unitOfWork.SaveChangesAsync();

    //    return _mapper.Map<PackageHistoryDto>(createdPackageHistory);
    //}

    public async Task<PackageHistoryDto> CreateAsync(CreatePackageHistoryDto dto)
    {
        // پیدا کردن پکیج
        var package = await _unitOfWork.Repository<Package>()
            .GetQueryable()
            .FirstOrDefaultAsync(p => p.Id == dto.PackageId);

        if (package == null)
            throw new KeyNotFoundException("Package not found");

        // پیدا کردن اکانت
        var account = await _unitOfWork.Repository<Account>()
            .GetQueryable()
            .Include(a => a.UserProfile) // فرض می‌کنیم UserProfile حاوی FirstName و LastName است
            .FirstOrDefaultAsync(a => a.Id == dto.AccountId);

        if (account == null)
            throw new KeyNotFoundException("Account not found");

        // ایجاد PackageHistory
        var packageHistory = new PackageHistory
        {
            PackageId = package.Id,
            AccountId = account.Id,
            Package = package,
            Account = account,
            CreatedAt = DateTime.UtcNow,
            ExpiredDate = DateTime.UtcNow.AddMonths(package.Duration), // مثال: Duration ماه
            RemainingTickets = package.TicketCount,
            RemainingMessages = package.MessageCount
        };

        // ذخیره در دیتابیس
        var created = await _unitOfWork.Repository<PackageHistory>().AddAsync(packageHistory);
        await _unitOfWork.SaveChangesAsync();

        // تبدیل به DTO
        var result = new PackageHistoryDto
        {
            Id = created.Id,
            PackageId = package.Id,
            PackageTitle = package.Title,
            PackagePrice = package.Price,
            RemainingTickets = created.RemainingTickets,
            RemainingMessages = created.RemainingMessages,
            ExpiredDate = created.ExpiredDate,
            AccountId = account.Id,
            UserFirstName = account.UserProfile?.FirstName,
            UserLastName = account.UserProfile?.LastName,
            UserEmail = account.Email,
            IsExpired = created.ExpiredDate < DateTime.UtcNow
        };

        return result;
    }


    /// <summary>
    /// Update existing package history
    /// </summary>
    public async Task<PackageHistoryDto> UpdateAsync(int id, PackageHistoryDto packageHistoryDto)
    {
        var existingPackageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id);
        if (existingPackageHistory == null)
            throw new ArgumentException($"Package history with ID {id} not found");

        // Update only allowed fields
        existingPackageHistory.ExpiredDate = packageHistoryDto.ExpiredDate;
        existingPackageHistory.RemainingTickets = packageHistoryDto.RemainingTickets;
        existingPackageHistory.RemainingMessages = packageHistoryDto.RemainingMessages;
        existingPackageHistory.PackageId = packageHistoryDto.PackageId;
        existingPackageHistory.AccountId = packageHistoryDto.AccountId;
        existingPackageHistory.UpdatedAt = DateTime.UtcNow;

        var updatedPackageHistory = await _unitOfWork.Repository<PackageHistory>().UpdateAsync(existingPackageHistory);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<PackageHistoryDto>(updatedPackageHistory);
    }

    /// <summary>
    /// Delete package history
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id);
        if (packageHistory == null)
            return false;

        await _unitOfWork.Repository<PackageHistory>().DeleteAsync(packageHistory.Id);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Update remaining tickets
    /// </summary>
    public async Task<PackageHistoryDto> UpdateRemainingTicketsAsync(int id, int remainingTickets)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id);
        if (packageHistory == null)
            throw new ArgumentException($"Package history with ID {id} not found");

        packageHistory.RemainingTickets = remainingTickets;
        packageHistory.UpdatedAt = DateTime.UtcNow;

        var updatedPackageHistory = await _unitOfWork.Repository<PackageHistory>().UpdateAsync(packageHistory);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<PackageHistoryDto>(updatedPackageHistory);
    }

    /// <summary>
    /// Update remaining messages
    /// </summary>
    public async Task<PackageHistoryDto> UpdateRemainingMessagesAsync(int id, int remainingMessages)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id);
        if (packageHistory == null)
            throw new ArgumentException($"Package history with ID {id} not found");

        packageHistory.RemainingMessages = remainingMessages;
        packageHistory.UpdatedAt = DateTime.UtcNow;

        var updatedPackageHistory = await _unitOfWork.Repository<PackageHistory>().UpdateAsync(packageHistory);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<PackageHistoryDto>(updatedPackageHistory);
    }

    /// <summary>
    /// Extend package expiration
    /// </summary>
    public async Task<PackageHistoryDto> ExtendExpirationAsync(int id, DateTime newExpiryDate)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id);
        if (packageHistory == null)
            throw new ArgumentException($"Package history with ID {id} not found");

        packageHistory.ExpiredDate = newExpiryDate;
        packageHistory.UpdatedAt = DateTime.UtcNow;

        var updatedPackageHistory = await _unitOfWork.Repository<PackageHistory>().UpdateAsync(packageHistory);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<PackageHistoryDto>(updatedPackageHistory);
    }

    /// <summary>
    /// Get package history statistics
    /// </summary>
    public async Task<PackageHistoryStatisticsDto> GetStatisticsAsync()
    {
        var allPackageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync();
        var currentDate = DateTime.UtcNow;
        
        var activePackages = allPackageHistories.Count(ph => ph.ExpiredDate >= currentDate);
        var expiredPackages = allPackageHistories.Count(ph => ph.ExpiredDate < currentDate);
        
        var packagesThisMonth = allPackageHistories.Count(ph => 
            ph.CreatedAt.Month == currentDate.Month && ph.CreatedAt.Year == currentDate.Year);
        var packagesLastMonth = allPackageHistories.Count(ph => 
            ph.CreatedAt.Month == currentDate.AddMonths(-1).Month && ph.CreatedAt.Year == currentDate.AddMonths(-1).Year);
        
        // Calculate totals
        var totalTicketsSold = allPackageHistories.Sum(ph => ph.RemainingTickets);
        var totalMessagesSold = allPackageHistories.Sum(ph => ph.RemainingMessages);

        // Calculate averages
        var averageTicketsPerPackage = allPackageHistories.Count() > 0
            ? (double)totalTicketsSold / allPackageHistories.Count()
            : 0;

        var averageMessagesPerPackage = allPackageHistories.Count() > 0
            ? (double)totalMessagesSold / allPackageHistories.Count()
            : 0;

        // Revenue calculations would need actual package price data
        var totalRevenue = 0m; // Placeholder
        var revenueThisMonth = 0m; // Placeholder
        var revenueLastMonth = 0m; // Placeholder
        
        return new PackageHistoryStatisticsDto
        {
            TotalPackageHistories = allPackageHistories.Count(),
            ActivePackages = activePackages,
            ExpiredPackages = expiredPackages,
            TotalRevenue = totalRevenue,
            TotalTicketsSold = totalTicketsSold,
            TotalMessagesSold = totalMessagesSold,
            TotalTicketsUsed = 0, // Would need to calculate from package data
            TotalMessagesUsed = 0, // Would need to calculate from package data
            AverageTicketsPerPackage = averageTicketsPerPackage,
            AverageMessagesPerPackage = averageMessagesPerPackage,
            PackagesThisMonth = packagesThisMonth,
            PackagesLastMonth = packagesLastMonth,
            RevenueThisMonth = revenueThisMonth,
            RevenueLastMonth = revenueLastMonth
        };
    }

    /// <summary>
    /// Get package histories by date range
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => 
            ph.CreatedAt >= startDate && ph.CreatedAt <= endDate);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get expiring packages (expiring within specified days)
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetExpiringPackagesAsync(int days = 7)
    {
        var currentDate = DateTime.UtcNow;
        var expiryThreshold = currentDate.AddDays(days);
        
        var expiringPackages = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => 
            ph.ExpiredDate >= currentDate && ph.ExpiredDate <= expiryThreshold);
        return expiringPackages.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get user package usage summary
    /// </summary>
    public async Task<UserPackageUsageSummaryDto> GetUserPackageUsageSummaryAsync(int accountId)
    {
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => ph.AccountId == accountId);
        var currentDate = DateTime.UtcNow;
        
        var activePackages = packageHistories.Count(ph => ph.ExpiredDate >= currentDate);
        var expiredPackages = packageHistories.Count(ph => ph.ExpiredDate < currentDate);
        
        var totalTicketsPurchased = packageHistories.Sum(ph => ph.RemainingTickets);
        var totalMessagesPurchased = packageHistories.Sum(ph => ph.RemainingMessages);
        
        var nextExpiryDate = packageHistories
            .Where(ph => ph.ExpiredDate >= currentDate)
            .OrderBy(ph => ph.ExpiredDate)
            .FirstOrDefault()?.ExpiredDate;
        
        var packageUsage = packageHistories.Select(ph => new PackageUsageDto
        {
            PackageId = ph.PackageId,
            PackageTitle = "", // Would need to get from Package entity
            PurchaseDate = ph.CreatedAt,
            ExpiryDate = ph.ExpiredDate,
            IsExpired = ph.ExpiredDate < currentDate,
            TotalTickets = ph.RemainingTickets, // Would need to get from Package entity
            TotalMessages = ph.RemainingMessages, // Would need to get from Package entity
            UsedTickets = 0, // Would need to calculate
            UsedMessages = 0, // Would need to calculate
            RemainingTickets = ph.RemainingTickets,
            RemainingMessages = ph.RemainingMessages,
            UsagePercentage = 0 // Would need to calculate
        }).ToList();
        
        return new UserPackageUsageSummaryDto
        {
            AccountId = accountId,
            UserFirstName = "", // Would need to get from Account entity
            UserLastName = "", // Would need to get from Account entity
            TotalPackagesPurchased = packageHistories.Count(),
            ActivePackages = activePackages,
            ExpiredPackages = expiredPackages,
            TotalSpent = 0, // Would need to calculate from package prices
            TotalTicketsPurchased = totalTicketsPurchased,
            TotalMessagesPurchased = totalMessagesPurchased,
            TotalTicketsUsed = 0, // Would need to calculate
            TotalMessagesUsed = 0, // Would need to calculate
            RemainingTickets = packageHistories.Sum(ph => ph.RemainingTickets),
            RemainingMessages = packageHistories.Sum(ph => ph.RemainingMessages),
            NextExpiryDate = nextExpiryDate,
            PackageUsage = packageUsage
        };
    }
}
