using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using System.Collections.Generic;

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
    public async Task<PaginatedResult<PackageHistoryDto>> GetAllAsync(int page = 1, int pageSize = 10, int? accountId = null, int? packageId = null, bool? isExpired = null, CancellationToken ct = default)
    {
        var query = _unitOfWork
            .Repository<PackageHistory>()
            .GetQueryable()
            .AsNoTracking();

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
        var totalCount = await _unitOfWork.Repository<PackageHistory>().CountAsync(query, ct);

        var dataQuery = query
            .Include(ph => ph.Package)
            .Include(ph => ph.Account)
            .ThenInclude(account => account.UserProfile);

        // Apply pagination
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetPagedAsync(dataQuery, page, pageSize, ct);

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
    public async Task<PackageHistoryDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id, ct);
        return packageHistory != null ? _mapper.Map<PackageHistoryDto>(packageHistory) : null;
    }

    /// <summary>
    /// Get package histories by account ID
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetByAccountAsync(int accountId, CancellationToken ct = default)
    {
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => ph.AccountId == accountId, ct: ct);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get package histories by package ID
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetByPackageAsync(int packageId, CancellationToken ct = default)
    {
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => ph.PackageId == packageId, ct: ct);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get active package histories for user
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetActiveByAccountAsync(int accountId, CancellationToken ct = default)
    {
        var currentDate = DateTime.UtcNow;
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph =>
            ph.AccountId == accountId && ph.ExpiredDate >= currentDate, ct: ct);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get expired package histories for user
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetExpiredByAccountAsync(int accountId, CancellationToken ct = default)
    {
        var currentDate = DateTime.UtcNow;
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph =>
            ph.AccountId == accountId && ph.ExpiredDate < currentDate, ct: ct);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Check if user has active package
    /// </summary>
    public async Task<bool> UserHasActivePackageAsync(int accountId, int packageId, CancellationToken ct = default)
    {
        var currentDate = DateTime.UtcNow;
        var activePackage = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph =>
            ph.AccountId == accountId &&
            ph.PackageId == packageId &&
            ph.ExpiredDate >= currentDate &&
            ph.RemainingTickets > 0, ct: ct);
        return activePackage.Any();
    }

    public async Task<PackageHistoryDto> CreateAsync(CreatePackageHistoryDto dto, CancellationToken ct = default)
    {
        var package = await _unitOfWork.Repository<Package>()
            .GetQueryable()
            .Include(p => p.DurationUnit)
            .FirstOrDefaultAsync(p => p.Id == dto.PackageId, ct);

        if (package == null)
            throw new KeyNotFoundException("Package not found");

        var account = await _unitOfWork.Repository<Account>()
            .GetQueryable()
            .Include(a => a.UserProfile)
            .FirstOrDefaultAsync(a => a.Id == dto.AccountId, ct);

        if (account == null)
            throw new KeyNotFoundException("Account not found");

        var now = DateTime.UtcNow;

        var existingActive = await _unitOfWork.Repository<PackageHistory>()
            .GetQueryable()
            .Include(ph => ph.Package)
            .Include(ph => ph.Account)
            .Where(ph => ph.AccountId == account.Id &&
                         ph.PackageId == package.Id &&
                         ph.ExpiredDate >= now)
            .OrderByDescending(ph => ph.ExpiredDate)
            .FirstOrDefaultAsync(ct);

        if (existingActive != null)
        {
            var baseDate = existingActive.ExpiredDate > now ? existingActive.ExpiredDate : now;

            existingActive.RemainingTickets += package.TicketCount;
            existingActive.RemainingMessages = 0;
            existingActive.ExpiredDate = CalculateExpiryDate(baseDate, package);
            existingActive.PurchasePrice += package.Price;
            existingActive.Package ??= package;
            existingActive.Account ??= account;

            var updated = await _unitOfWork.Repository<PackageHistory>().UpdateAsync(existingActive, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return MapToDto(updated, account);
        }

        var packageHistory = new PackageHistory
        {
            PackageId = package.Id,
            AccountId = account.Id,
            Package = package,
            Account = account,
            ExpiredDate = CalculateExpiryDate(now, package),
            RemainingTickets = package.TicketCount,
            RemainingMessages = 0,
            PurchasePrice = package.Price
        };

        var created = await _unitOfWork.Repository<PackageHistory>().AddAsync(packageHistory, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return MapToDto(created, account);
    }


    /// <summary>
    /// Update existing package history
    /// </summary>
    public async Task<PackageHistoryDto> UpdateAsync(int id, PackageHistoryDto packageHistoryDto, CancellationToken ct = default)
    {
        var existingPackageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id, ct);
        if (existingPackageHistory == null)
            throw new ArgumentException($"Package history with ID {id} not found");

        // Update only allowed fields
        existingPackageHistory.ExpiredDate = packageHistoryDto.ExpiredDate;
        existingPackageHistory.RemainingTickets = packageHistoryDto.RemainingTickets;
        existingPackageHistory.RemainingMessages = 0;
        existingPackageHistory.PackageId = packageHistoryDto.PackageId;
        existingPackageHistory.AccountId = packageHistoryDto.AccountId;
        existingPackageHistory.PurchasePrice = packageHistoryDto.PackagePrice;

        var updatedPackageHistory = await _unitOfWork.Repository<PackageHistory>().UpdateAsync(existingPackageHistory, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<PackageHistoryDto>(updatedPackageHistory);
    }

    /// <summary>
    /// Delete package history
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id, ct);
        if (packageHistory == null)
            return false;

        await _unitOfWork.Repository<PackageHistory>().DeleteAsync(packageHistory.Id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    /// <summary>
    /// Update remaining tickets
    /// </summary>
    public async Task<PackageHistoryDto> UpdateRemainingTicketsAsync(int id, int remainingTickets, CancellationToken ct = default)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id, ct);
        if (packageHistory == null)
            throw new ArgumentException($"Package history with ID {id} not found");

        packageHistory.RemainingTickets = remainingTickets;

        var updatedPackageHistory = await _unitOfWork.Repository<PackageHistory>().UpdateAsync(packageHistory, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<PackageHistoryDto>(updatedPackageHistory);
    }

    /// <summary>
    /// Update remaining messages
    /// </summary>
    public async Task<PackageHistoryDto> UpdateRemainingMessagesAsync(int id, int remainingMessages, CancellationToken ct = default)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id, ct);
        if (packageHistory == null)
            throw new ArgumentException($"Package history with ID {id} not found");

        packageHistory.RemainingMessages = remainingMessages;

        var updatedPackageHistory = await _unitOfWork.Repository<PackageHistory>().UpdateAsync(packageHistory, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<PackageHistoryDto>(updatedPackageHistory);
    }

    /// <summary>
    /// Extend package expiration
    /// </summary>
    public async Task<PackageHistoryDto> ExtendExpirationAsync(int id, DateTime newExpiryDate, CancellationToken ct = default)
    {
        var packageHistory = await _unitOfWork.Repository<PackageHistory>().GetByIdAsync(id, ct);
        if (packageHistory == null)
            throw new ArgumentException($"Package history with ID {id} not found");

        packageHistory.ExpiredDate = newExpiryDate;

        var updatedPackageHistory = await _unitOfWork.Repository<PackageHistory>().UpdateAsync(packageHistory, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<PackageHistoryDto>(updatedPackageHistory);
    }

    /// <summary>
    /// Get package history statistics
    /// </summary>
    public async Task<PackageHistoryStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        var allPackageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ct: ct);
        var currentDate = DateTime.UtcNow;

        var activePackages = allPackageHistories.Count(ph => ph.ExpiredDate >= currentDate);
        var expiredPackages = allPackageHistories.Count(ph => ph.ExpiredDate < currentDate);

        var packagesThisMonth = allPackageHistories.Count(ph =>
            ph.CreatedAt.Month == currentDate.Month && ph.CreatedAt.Year == currentDate.Year);
        var packagesLastMonth = allPackageHistories.Count(ph =>
            ph.CreatedAt.Month == currentDate.AddMonths(-1).Month && ph.CreatedAt.Year == currentDate.AddMonths(-1).Year);

        // Calculate totals
        var totalTicketsSold = allPackageHistories.Sum(ph => ph.RemainingTickets);
        var totalMessagesSold = 0;

        // Calculate averages
        var averageTicketsPerPackage = allPackageHistories.Count() > 0
            ? (double)totalTicketsSold / allPackageHistories.Count()
            : 0;

        var averageMessagesPerPackage = 0d;

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
            TotalMessagesUsed = 0, // No message limits
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
    public async Task<IEnumerable<PackageHistoryDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph =>
            ph.CreatedAt >= startDate && ph.CreatedAt <= endDate, ct: ct);
        return packageHistories.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get expiring packages (expiring within specified days)
    /// </summary>
    public async Task<IEnumerable<PackageHistoryDto>> GetExpiringPackagesAsync(int days = 7, CancellationToken ct = default)
    {
        var currentDate = DateTime.UtcNow;
        var expiryThreshold = currentDate.AddDays(days);

        var expiringPackages = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph =>
            ph.ExpiredDate >= currentDate && ph.ExpiredDate <= expiryThreshold, ct: ct);
        return expiringPackages.Select(ph => _mapper.Map<PackageHistoryDto>(ph));
    }

    /// <summary>
    /// Get user package usage summary
    /// </summary>
    public async Task<UserPackageUsageSummaryDto> GetUserPackageUsageSummaryAsync(int accountId, CancellationToken ct = default)
    {
        var packageHistories = await _unitOfWork.Repository<PackageHistory>().GetAllAsync(ph => ph.AccountId == accountId, ct: ct);
        var currentDate = DateTime.UtcNow;

        var activePackages = packageHistories.Count(ph => ph.ExpiredDate >= currentDate);
        var expiredPackages = packageHistories.Count(ph => ph.ExpiredDate < currentDate);

        var totalTicketsPurchased = packageHistories.Sum(ph => ph.RemainingTickets);
        var totalMessagesPurchased = 0;

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
            TotalMessages = 0,
            UsedTickets = 0, // Would need to calculate
            UsedMessages = 0, // No message limits
            RemainingTickets = ph.RemainingTickets,
            RemainingMessages = 0,
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
            TotalMessagesUsed = 0, // No message limits
            RemainingTickets = packageHistories.Sum(ph => ph.RemainingTickets),
            RemainingMessages = 0,
            NextExpiryDate = nextExpiryDate,
            PackageUsage = packageUsage
        };
    }

    private static DateTime CalculateExpiryDate(DateTime startDate, Package package)
    {
        return package.DurationUnit?.Key switch
        {
            "Day" => startDate.AddDays(package.Duration),
            "Week" => startDate.AddDays(7 * package.Duration),
            _ => startDate.AddMonths(package.Duration)
        };
    }

    private PackageHistoryDto MapToDto(PackageHistory history, Account account)
    {
        return new PackageHistoryDto
        {
            Id = history.Id,
            PackageId = history.PackageId,
            PackageTitle = history.Package?.Title,
            PackagePrice = history.PurchasePrice,
            RemainingTickets = history.RemainingTickets,
            RemainingMessages = 0,
            ExpiredDate = history.ExpiredDate,
            AccountId = history.AccountId,
            UserFirstName = account.UserProfile?.FirstName,
            UserLastName = account.UserProfile?.LastName,
            UserEmail = account.Email,
            IsExpired = history.ExpiredDate < DateTime.UtcNow,
            TotalTickets = history.Package?.TicketCount ?? history.RemainingTickets,
            TotalMessages = 0
        };
    }
}
