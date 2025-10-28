using System;
using System.Collections.Generic;
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
    /// <param name="expiresAfter">Filter packages expiring after this date</param>
    /// <param name="expiresBefore">Filter packages expiring before this date</param>
    /// <returns>Paginated list of packages</returns>
    Task<PaginatedResult<PackageDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, decimal? minPrice = null, decimal? maxPrice = null, DateTime? expiresAfter = null, DateTime? expiresBefore = null);
    
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
    /// Create new package
    /// </summary>
    /// <param name="packageDto">Package data</param>
    /// <returns>Created package</returns>
    Task<PackageDto> CreateAsync(CreatePackageDto packageDto);
    
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
    /// Update package credit allowance
    /// </summary>
    /// <param name="id">Package ID</param>
    /// <param name="creditCount">New credit count</param>
    /// <returns>Updated package</returns>
    Task<PackageDto> UpdateCreditsAsync(int id, int creditCount);

    /// <summary>
    /// Update package expiration
    /// </summary>
    /// <param name="id">Package ID</param>
    /// <param name="expirationDate">New expiration date</param>
    /// <returns>Updated package</returns>
    Task<PackageDto> UpdateExpirationAsync(int id, DateTime expirationDate);
}

/// <summary>
/// Package statistics DTO
/// </summary>
public class PackageStatisticsDto
{
    public int TotalPackages { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalCreditsSold { get; set; }
    public int TotalCreditsUsed { get; set; }
    public double AveragePackagePrice { get; set; }
    public int PackagesThisMonth { get; set; }
    public int PackagesLastMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
}
