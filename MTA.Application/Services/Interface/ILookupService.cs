using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface ILookupService
{
    Task<PaginatedResult<LookupDto>> GetAllAsync(int page = 1, int pageSize = 10, string? category = null, string? searchTerm = null, CancellationToken ct = default);
    Task<LookupDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<LookupDto>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<LookupDto?> GetByCategoryAndKeyAsync(string category, string key, CancellationToken ct = default);
    Task<string?> GetValueAsync(string category, string key, CancellationToken ct = default);
    Task<LookupDto> CreateAsync(CreateLookupDto lookupDto, CancellationToken ct = default);
    Task<LookupDto> UpdateAsync(int id, LookupDto lookupDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<string>> GetAllCategoriesAsync(CancellationToken ct = default);
    Task<LookupStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    Task<IEnumerable<LookupDto>> BulkCreateAsync(IEnumerable<LookupDto> lookupDtos, CancellationToken ct = default);
}

public class LookupStatisticsDto
{
    public int TotalLookups { get; set; }
    public int TotalCategories { get; set; }
    public Dictionary<string, int> LookupsPerCategory { get; set; } = new();
    public int LookupsThisMonth { get; set; }
    public int LookupsLastMonth { get; set; }
}
