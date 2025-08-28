using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Lookup operations
/// </summary>
public interface ILookupService
{
    /// <summary>
    /// Get all lookups with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="category">Filter by category</param>
    /// <param name="searchTerm">Search term for key or value</param>
    /// <returns>Paginated list of lookups</returns>
    Task<PaginatedResult<LookupDto>> GetAllAsync(int page = 1, int pageSize = 10, string? category = null, string? searchTerm = null);
    
    /// <summary>
    /// Get lookup by ID
    /// </summary>
    /// <param name="id">Lookup ID</param>
    /// <returns>Lookup details</returns>
    Task<LookupDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get lookups by category
    /// </summary>
    /// <param name="category">Category name</param>
    /// <returns>List of lookups</returns>
    Task<IEnumerable<LookupDto>> GetByCategoryAsync(string category);
    
    /// <summary>
    /// Get lookup by category and key
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="key">Key name</param>
    /// <returns>Lookup details</returns>
    Task<LookupDto?> GetByCategoryAndKeyAsync(string category, string key);
    
    /// <summary>
    /// Get lookup value by category and key
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="key">Key name</param>
    /// <returns>Lookup value</returns>
    Task<string?> GetValueAsync(string category, string key);
    
    /// <summary>
    /// Create new lookup
    /// </summary>
    /// <param name="lookupDto">Lookup data</param>
    /// <returns>Created lookup</returns>
    Task<LookupDto> CreateAsync(LookupDto lookupDto);
    
    /// <summary>
    /// Update existing lookup
    /// </summary>
    /// <param name="id">Lookup ID</param>
    /// <param name="lookupDto">Updated lookup data</param>
    /// <returns>Updated lookup</returns>
    Task<LookupDto> UpdateAsync(int id, LookupDto lookupDto);
    
    /// <summary>
    /// Delete lookup
    /// </summary>
    /// <param name="id">Lookup ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Get all categories
    /// </summary>
    /// <returns>List of unique categories</returns>
    Task<IEnumerable<string>> GetAllCategoriesAsync();
    
    /// <summary>
    /// Get lookup statistics
    /// </summary>
    /// <returns>Lookup statistics</returns>
    Task<LookupStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Bulk create lookups
    /// </summary>
    /// <param name="lookupDtos">List of lookup data</param>
    /// <returns>List of created lookups</returns>
    Task<IEnumerable<LookupDto>> BulkCreateAsync(IEnumerable<LookupDto> lookupDtos);
}

/// <summary>
/// Lookup statistics DTO
/// </summary>
public class LookupStatisticsDto
{
    public int TotalLookups { get; set; }
    public int TotalCategories { get; set; }
    public Dictionary<string, int> LookupsPerCategory { get; set; } = new();
    public int LookupsThisMonth { get; set; }
    public int LookupsLastMonth { get; set; }
}
