using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Package operations
/// </summary>
public interface IPackageService
{
    /// <summary>
    /// Get all packages with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="durationUnitId">Filter by duration unit ID</param>
    /// <returns>Paginated list of packages</returns>
    Task<PaginatedResult<PackageDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, decimal? minPrice = null, decimal? maxPrice = null, int? durationUnitId = null);
    
    /// <summary>
    /// Get package by ID
    /// </summary>
    /// <param name="id">Package ID</param>
    /// <returns>Package details</returns>
    Task<PackageDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get packages by price range
    /// </summary>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <returns>List of packages</returns>
    Task<IEnumerable<PackageDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    
    /// <summary>
    /// Get packages by duration unit
    /// </summary>
    /// <param name="durationUnitId">Duration unit ID</param>
    /// <returns>List of packages</returns>
    Task<IEnumerable<PackageDto>> GetByDurationUnitAsync(int durationUnitId);
    
    /// <summary>
    /// Create new package
    /// </summary>
    /// <param name="packageDto">Package data</param>
    /// <returns>Created package</returns>
    Task<PackageDto> CreateAsync(PackageDto packageDto);
    
    /// <summary>
    /// Update existing package
    /// </summary>
    /// <param name="id">Package ID</param>
    /// <param name="packageDto">Updated package data</param>
    /// <returns>Updated package</returns>
    Task<PackageDto> UpdateAsync(int id, PackageDto packageDto);
    
    /// <summary>
    /// Delete package
    /// </summary>
    /// <param name="id">Package ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Update package price
    /// </summary>
    /// <param name="id">Package ID</param>
    /// <param name="price">New price</param>
    /// <returns>Updated package</returns>
    Task<PackageDto> UpdatePriceAsync(int id, decimal price);
    
    /// <summary>
    /// Update package capacity
    /// </summary>
    /// <param name="id">Package ID</param>
    /// <param name="ticketCount">New ticket count</param>
    /// <param name="messageCount">New message count</param>
    /// <returns>Updated package</returns>
    Task<PackageDto> UpdateCapacityAsync(int id, int ticketCount, int messageCount);
    
    /// <summary>
    /// Update package duration
    /// </summary>
    /// <param name="id">Package ID</param>
    /// <param name="duration">New duration</param>
    /// <param name="durationUnitId">New duration unit ID</param>
    /// <returns>Updated package</returns>
    Task<PackageDto> UpdateDurationAsync(int id, int duration, int durationUnitId);
}

/// <summary>
/// Package statistics DTO
/// </summary>
public class PackageStatisticsDto
{
    public int TotalPackages { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalTicketsSold { get; set; }
    public int TotalMessagesSold { get; set; }
    public double AveragePackagePrice { get; set; }
    public int PackagesThisMonth { get; set; }
    public int PackagesLastMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
}
