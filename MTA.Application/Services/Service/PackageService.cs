using System;
using System.Linq;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for Package operations aligned with credit-based usage.
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

    /// <inheritdoc />
    public async Task<PaginatedResult<PackageDto>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        DateTime? expiresAfter = null,
        DateTime? expiresBefore = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Package>().GetQueryable()
                .Include(p => p.Tickets)
                .Include(p => p.PackageHistories)
                .AsQueryable();

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

            if (expiresAfter.HasValue)
            {
                query = query.Where(p => p.ExpirationDate >= expiresAfter.Value);
            }

            if (expiresBefore.HasValue)
            {
                query = query.Where(p => p.ExpirationDate <= expiresBefore.Value);
            }

            var totalCount = await query.CountAsync();

            var packages = await query
                .OrderByDescending(p => p.ExpirationDate)
                .ThenBy(p => p.Price)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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
            _logger.LogError(
                ex,
                "Error getting packages with filters {@Filters}",
                new { page, pageSize, searchTerm, minPrice, maxPrice, expiresAfter, expiresBefore });
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PackageDto?> GetByIdAsync(int id)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetQueryable()
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

    /// <inheritdoc />
    public async Task<IEnumerable<PackageDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        try
        {
            var packages = await _unitOfWork.Repository<Package>().GetQueryable()
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

    /// <inheritdoc />
    public async Task<PackageDto> CreateAsync(CreatePackageDto packageDto)
    {
        try
        {
            if (packageDto.CreditCount < 0)
            {
                throw new InvalidOperationException("Credit count cannot be negative");
            }

            if (packageDto.ExpirationDate <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Expiration date must be in the future");
            }

            var package = new Package
            {
                Title = packageDto.Title,
                Price = packageDto.Price,
                CreditCount = packageDto.CreditCount,
                ExpirationDate = packageDto.ExpirationDate
            };

            await _unitOfWork.Repository<Package>().AddAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(package.Id) ?? MapToDto(package);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating package with title: {Title}", packageDto.Title);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PackageDto> UpdateAsync(int id, PackageDto packageDto)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {id} not found");
            }

            if (packageDto.CreditCount < 0)
            {
                throw new InvalidOperationException("Credit count cannot be negative");
            }

            if (packageDto.ExpirationDate <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Expiration date must be in the future");
            }

            package.Title = packageDto.Title;
            package.Price = packageDto.Price;
            package.CreditCount = packageDto.CreditCount;
            package.ExpirationDate = packageDto.ExpirationDate;
            package.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().UpdateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id) ?? MapToDto(package);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            if (package == null)
            {
                return false;
            }

            var hasTickets = await _unitOfWork.Repository<Ticket>().GetQueryable()
                .AnyAsync(t => t.PackageId == id);

            var hasHistory = await _unitOfWork.Repository<PackageHistory>().GetQueryable()
                .AnyAsync(ph => ph.PackageId == id);

            if (hasTickets || hasHistory)
            {
                throw new InvalidOperationException(
                    $"Cannot delete package '{package.Title}' as it has associated tickets or purchase history");
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

    /// <inheritdoc />
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

            return await GetByIdAsync(id) ?? MapToDto(package);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating price for package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PackageDto> UpdateCreditsAsync(int id, int creditCount)
    {
        try
        {
            if (creditCount < 0)
            {
                throw new InvalidOperationException("Credit count cannot be negative");
            }

            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {id} not found");
            }

            package.CreditCount = creditCount;
            package.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().UpdateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id) ?? MapToDto(package);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating credit count for package with ID: {PackageId}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PackageDto> UpdateExpirationAsync(int id, DateTime expirationDate)
    {
        try
        {
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {id} not found");
            }

            if (expirationDate <= DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException("Expiration date must be in the future");
            }

            package.ExpirationDate = expirationDate;
            package.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Package>().UpdateAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id) ?? MapToDto(package);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating expiration for package with ID: {PackageId}", id);
            throw;
        }
    }

    private PackageDto MapToDto(Package package)
    {
        return new PackageDto
        {
            Id = package.Id,
            Title = package.Title,
            Price = package.Price,
            CreditCount = package.CreditCount,
            ExpirationDate = package.ExpirationDate,
            UsedCreditCount = package.Tickets?.Count ?? 0,
            CreatedAt = package.CreatedAt,
            UpdatedAt = package.UpdatedAt
        };
    }
}
