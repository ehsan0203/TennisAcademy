using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for Package operations
/// </summary>
public class PackageService : IPackageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PackageService> _logger;

    public PackageService(IUnitOfWork unitOfWork, ILogger<PackageService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all packages with optional filtering
    /// </summary>
    public async Task<PaginatedResult<PackageDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, decimal? minPrice = null, decimal? maxPrice = null, int? durationUnitId = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Package>().GetQueryable()
                .Include(p => p.DurationUnit)
                .Include(p => p.Tickets)
                .Include(p => p.PackageHistories)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Title.Contains(searchTerm));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (durationUnitId.HasValue)
            {
                query = query.Where(p => p.DurationUnitId == durationUnitId.Value);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination and ordering
            var packages = await query
                .OrderBy(p => p.Price)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to DTOs
            var packageDtos = packages.Select(MapToDto).ToList();

            return new PaginatedResult<PackageDto>
            {
                Data = packageDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting packages with page {Page}, pageSize {PageSize}, searchTerm {SearchTerm}, minPrice {MinPrice}, maxPrice {MaxPrice}, durationUnitId {DurationUnitId}", 
                page, pageSize, searchTerm, minPrice, maxPrice, durationUnitId);
            throw;
        }
    }

    /// <summary>
    /// Get package by ID
    /// </summary>
    public async Task<PackageDto?> GetByIdAsync(int id)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetQueryable()
                .Include(p => p.DurationUnit)
                .Include(p => p.Tickets)
                .Include(p => p.PackageHistories)
                .FirstOrDefaultAsync(p => p.Id == id);

            return package != null ? MapToDto(package) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get packages by price range
    /// </summary>
    public async Task<IEnumerable<PackageDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        try
        {
            var packages = await _unitOfWork.Repository<Package>().GetQueryable()
                .Include(p => p.DurationUnit)
                .Include(p => p.Tickets)
                .Include(p => p.PackageHistories)
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .OrderBy(p => p.Price)
                .ToListAsync();

            return packages.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting packages by price range from {MinPrice} to {MaxPrice}", minPrice, maxPrice);
            throw;
        }
    }

    /// <summary>
    /// Get packages by duration unit
    /// </summary>
    public async Task<IEnumerable<PackageDto>> GetByDurationUnitAsync(int durationUnitId)
    {
        try
        {
            var packages = await _unitOfWork.Repository<Package>().GetQueryable()
                .Include(p => p.DurationUnit)
                .Include(p => p.Tickets)
                .Include(p => p.PackageHistories)
                .Where(p => p.DurationUnitId == durationUnitId)
                .OrderBy(p => p.Price)
                .ToListAsync();

            return packages.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting packages by duration unit ID: {DurationUnitId}", durationUnitId);
            throw;
        }
    }

    /// <summary>
    /// Create new package
    /// </summary>
    public async Task<PackageDto> CreateAsync(PackageDto packageDto)
    {
        try
        {
            // Validate duration unit exists
            var durationUnit = await _unitOfWork.Repository<Lookup>().GetByIdAsync(packageDto.DurationUnitId);
            if (durationUnit == null)
            {
                throw new InvalidOperationException($"Duration unit with ID {packageDto.DurationUnitId} not found");
            }

            var package = new Package
            {
                Title = packageDto.Title,
                Price = packageDto.Price,
                TicketCount = packageDto.TicketCount,
                MessageCount = packageDto.MessageCount,
                Duration = packageDto.Duration,
                DurationUnitId = packageDto.DurationUnitId
            };

            await _unitOfWork.Repository<Package>().AddAsync(package);
            await _unitOfWork.SaveChangesAsync();

            // Return the created package
            return await GetByIdAsync(package.Id) ?? packageDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating package with title: {Title}", packageDto.Title);
            throw;
        }
    }

    /// <summary>
    /// Update existing package
    /// </summary>
    public async Task<PackageDto> UpdateAsync(int id, PackageDto packageDto)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {id} not found");
            }

            // Validate duration unit exists if changing
            if (packageDto.DurationUnitId != package.DurationUnitId)
            {
                var durationUnit = await _unitOfWork.Repository<Lookup>().GetByIdAsync(packageDto.DurationUnitId);
                if (durationUnit == null)
                {
                    throw new InvalidOperationException($"Duration unit with ID {packageDto.DurationUnitId} not found");
                }
            }

            // Update properties
            package.Title = packageDto.Title;
            package.Price = packageDto.Price;
            package.TicketCount = packageDto.TicketCount;
            package.MessageCount = packageDto.MessageCount;
            package.Duration = packageDto.Duration;
            package.DurationUnitId = packageDto.DurationUnitId;
            package.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().UpdateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            // Return the updated package
            return await GetByIdAsync(id) ?? packageDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete package
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            if (package == null)
            {
                return false;
            }

            // Check if package has associated tickets or purchase history
            var hasTickets = await _unitOfWork.Repository<Ticket>().GetQueryable()
                .AnyAsync(t => t.PackageId == id);

            var hasHistory = await _unitOfWork.Repository<PackageHistory>().GetQueryable()
                .AnyAsync(ph => ph.PackageId == id);

            if (hasTickets || hasHistory)
            {
                throw new InvalidOperationException($"Cannot delete package '{package.Title}' as it has associated tickets or purchase history");
            }

            await _unitOfWork.Repository<Package>().DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Update package price
    /// </summary>
    public async Task<PackageDto> UpdatePriceAsync(int id, decimal price)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {id} not found");
            }

            if (price < 0)
            {
                throw new InvalidOperationException("Price cannot be negative");
            }

            package.Price = price;
            package.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().UpdateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id) ?? throw new InvalidOperationException("Failed to retrieve updated package");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating price for package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Update package capacity
    /// </summary>
    public async Task<PackageDto> UpdateCapacityAsync(int id, int ticketCount, int messageCount)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {id} not found");
            }

            if (ticketCount < 0 || messageCount < 0)
            {
                throw new InvalidOperationException("Ticket count and message count cannot be negative");
            }

            package.TicketCount = ticketCount;
            package.MessageCount = messageCount;
            package.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().UpdateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id) ?? throw new InvalidOperationException("Failed to retrieve updated package");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating capacity for package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Update package duration
    /// </summary>
    public async Task<PackageDto> UpdateDurationAsync(int id, int duration, int durationUnitId)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {id} not found");
            }

            // Validate duration unit exists
            var durationUnit = await _unitOfWork.Repository<Lookup>().GetByIdAsync(durationUnitId);
            if (durationUnit == null)
            {
                throw new InvalidOperationException($"Duration unit with ID {durationUnitId} not found");
            }

            if (duration <= 0)
            {
                throw new InvalidOperationException("Duration must be positive");
            }

            package.Duration = duration;
            package.DurationUnitId = durationUnitId;
            package.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().UpdateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id) ?? throw new InvalidOperationException("Failed to retrieve updated package");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating duration for package with ID: {PackageId}", id);
            throw;
        }
    }

    #region Helper Methods

    /// <summary>
    /// Map Package entity to PackageDto
    /// </summary>
    private PackageDto MapToDto(Package package)
    {
        return new PackageDto
        {
            Id = package.Id,
            Title = package.Title,
            Price = package.Price,
            TicketCount = package.TicketCount,
            MessageCount = package.MessageCount,
            Duration = package.Duration,
            DurationUnitId = package.DurationUnitId,
            DurationUnitValue = package.DurationUnit?.Value,
            UsedTicketCount = package.Tickets?.Count ?? 0,
            UsedMessageCount = package.Tickets?.Sum(t => t.Messages?.Count ?? 0) ?? 0,
            CreatedAt = package.CreatedAt,
            UpdatedAt = package.UpdatedAt
        };
    }

    #endregion
}
