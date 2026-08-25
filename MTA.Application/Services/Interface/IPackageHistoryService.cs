using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IPackageHistoryService
{
    Task<PaginatedResult<PackageHistoryDto>> GetAllAsync(int page = 1, int pageSize = 10, int? accountId = null, int? packageId = null, bool? isExpired = null, CancellationToken ct = default);
    Task<PackageHistoryDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<PackageHistoryDto>> GetByAccountAsync(int accountId, CancellationToken ct = default);
    Task<IEnumerable<PackageHistoryDto>> GetByPackageAsync(int packageId, CancellationToken ct = default);
    Task<IEnumerable<PackageHistoryDto>> GetActiveByAccountAsync(int accountId, CancellationToken ct = default);
    Task<IEnumerable<PackageHistoryDto>> GetExpiredByAccountAsync(int accountId, CancellationToken ct = default);
    Task<bool> UserHasActivePackageAsync(int accountId, int packageId, CancellationToken ct = default);
    Task<PackageHistoryDto> CreateAsync(CreatePackageHistoryDto packageHistoryDto, CancellationToken ct = default);
    Task<PackageHistoryDto> UpdateAsync(int id, PackageHistoryDto packageHistoryDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<PackageHistoryDto> UpdateRemainingTicketsAsync(int id, int remainingTickets, CancellationToken ct = default);
    Task<PackageHistoryDto> UpdateRemainingMessagesAsync(int id, int remainingMessages, CancellationToken ct = default);
    Task<PackageHistoryDto> ExtendExpirationAsync(int id, DateTime newExpiryDate, CancellationToken ct = default);
    Task<PackageHistoryStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    Task<IEnumerable<PackageHistoryDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<IEnumerable<PackageHistoryDto>> GetExpiringPackagesAsync(int days = 7, CancellationToken ct = default);
    Task<UserPackageUsageSummaryDto> GetUserPackageUsageSummaryAsync(int accountId, CancellationToken ct = default);
}

public class PackageHistoryStatisticsDto
{
    public int TotalPackageHistories { get; set; }
    public int ActivePackages { get; set; }
    public int ExpiredPackages { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalTicketsSold { get; set; }
    public int TotalMessagesSold { get; set; }
    public int TotalTicketsUsed { get; set; }
    public int TotalMessagesUsed { get; set; }
    public double AverageTicketsPerPackage { get; set; }
    public double AverageMessagesPerPackage { get; set; }
    public int PackagesThisMonth { get; set; }
    public int PackagesLastMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
}

public class UserPackageUsageSummaryDto
{
    public int AccountId { get; set; }
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public int TotalPackagesPurchased { get; set; }
    public int ActivePackages { get; set; }
    public int ExpiredPackages { get; set; }
    public decimal TotalSpent { get; set; }
    public int TotalTicketsPurchased { get; set; }
    public int TotalMessagesPurchased { get; set; }
    public int TotalTicketsUsed { get; set; }
    public int TotalMessagesUsed { get; set; }
    public int RemainingTickets { get; set; }
    public int RemainingMessages { get; set; }
    public DateTime? NextExpiryDate { get; set; }
    public List<PackageUsageDto> PackageUsage { get; set; } = new();
}

public class PackageUsageDto
{
    public int PackageId { get; set; }
    public string PackageTitle { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public int TotalTickets { get; set; }
    public int TotalMessages { get; set; }
    public int UsedTickets { get; set; }
    public int UsedMessages { get; set; }
    public int RemainingTickets { get; set; }
    public int RemainingMessages { get; set; }
    public double UsagePercentage { get; set; }
}
