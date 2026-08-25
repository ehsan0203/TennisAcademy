using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IPackageService
{
    Task<PaginatedResult<PackageDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, decimal? minPrice = null, decimal? maxPrice = null, int? durationUnitId = null, CancellationToken ct = default);
    Task<PackageDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<PackageDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken ct = default);
    Task<IEnumerable<PackageDto>> GetByDurationUnitAsync(int durationUnitId, CancellationToken ct = default);
    Task<PackageDto> CreateAsync(CreatePackageDto packageDto, CancellationToken ct = default);
    Task<PackageDto> UpdateAsync(int id, PackageDto packageDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<PackageDto> UpdatePriceAsync(int id, decimal price, CancellationToken ct = default);
    Task<PackageDto> UpdateCapacityAsync(int id, int ticketCount, int messageCount, CancellationToken ct = default);
    Task<PackageDto> UpdateDurationAsync(int id, int duration, int durationUnitId, CancellationToken ct = default);
}
