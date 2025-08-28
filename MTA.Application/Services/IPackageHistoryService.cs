using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for PackageHistory operations
/// </summary>
public interface IPackageHistoryService
{
    /// <summary>
    /// Get all package histories with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="accountId">Filter by account ID</param>
    /// <param name="packageId">Filter by package ID</param>
    /// <param name="isExpired">Filter by expired status</param>
    /// <returns>Paginated list of package histories</returns>
    Task<PaginatedResult<PackageHistoryDto>> GetAllAsync(int page = 1, int pageSize = 10, int? accountId = null, int? packageId = null, bool? isExpired = null);
    
    /// <summary>
    /// Get package history by ID
    /// </summary>
    /// <param name="id">Package history ID</param>
    /// <returns>Package history details</returns>
    Task<PackageHistoryDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get package histories by account ID
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <returns>List of package histories</returns>
    Task<IEnumerable<PackageHistoryDto>> GetByAccountAsync(int accountId);
    
    /// <summary>
    /// Get package histories by package ID
    /// </summary>
    /// <param name="packageId">Package ID</param>
    /// <returns>List of package histories</returns>
    Task<IEnumerable<PackageHistoryDto>> GetByPackageAsync(int packageId);
    
    /// <summary>
    /// Get active package histories for user
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <returns>List of active package histories</returns>
    Task<IEnumerable<PackageHistoryDto>> GetActiveByAccountAsync(int accountId);
    
    /// <summary>
    /// Get expired package histories for user
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <returns>List of expired package histories</returns>
    Task<IEnumerable<PackageHistoryDto>> GetExpiredByAccountAsync(int accountId);
    
    /// <summary>
    /// Check if user has active package
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <param name="packageId">Package ID</param>
    /// <returns>True if user has active package</returns>
    Task<bool> UserHasActivePackageAsync(int accountId, int packageId);
    
    /// <summary>
    /// Create new package history
    /// </summary>
    /// <param name="packageHistoryDto">Package history data</param>
    /// <returns>Created package history</returns>
    Task<PackageHistoryDto> CreateAsync(PackageHistoryDto packageHistoryDto);
    
    /// <summary>
    /// Update existing package history
    /// </summary>
    /// <param name="id">Package history ID</param>
    /// <param name="packageHistoryDto">Updated package history data</param>
    /// <returns>Updated package history</returns>
    Task<PackageHistoryDto> UpdateAsync(int id, PackageHistoryDto packageHistoryDto);
    
    /// <summary>
    /// Delete package history
    /// </summary>
    /// <param name="id">Package history ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Update remaining tickets
    /// </summary>
    /// <param name="id">Package history ID</param>
    /// <param name="remainingTickets">New remaining tickets count</param>
    /// <returns>Updated package history</returns>
    Task<PackageHistoryDto> UpdateRemainingTicketsAsync(int id, int remainingTickets);
    
    /// <summary>
    /// Update remaining messages
    /// </summary>
    /// <param name="id">Package history ID</param>
    /// <param name="remainingMessages">New remaining messages count</param>
    /// <returns>Updated package history</returns>
    Task<PackageHistoryDto> UpdateRemainingMessagesAsync(int id, int remainingMessages);
    
    /// <summary>
    /// Extend package expiration
    /// </summary>
    /// <param name="id">Package history ID</param>
    /// <param name="newExpiryDate">New expiration date</param>
    /// <returns>Updated package history</returns>
    Task<PackageHistoryDto> ExtendExpirationAsync(int id, DateTime newExpiryDate);
    
    /// <summary>
    /// Get package history statistics
    /// </summary>
    /// <returns>Package history statistics</returns>
    Task<PackageHistoryStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get package histories by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of package histories</returns>
    Task<IEnumerable<PackageHistoryDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Get expiring packages (expiring within specified days)
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <returns>List of expiring packages</returns>
    Task<IEnumerable<PackageHistoryDto>> GetExpiringPackagesAsync(int days = 7);
    
    /// <summary>
    /// Get user package usage summary
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <returns>User package usage summary</returns>
    Task<UserPackageUsageSummaryDto> GetUserPackageUsageSummaryAsync(int accountId);
}

/// <summary>
/// Package history statistics DTO
/// </summary>
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

/// <summary>
/// User package usage summary DTO
/// </summary>
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

/// <summary>
/// Package usage DTO
/// </summary>
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
    public double UsagePercentage { get; set; } // percentage
}
