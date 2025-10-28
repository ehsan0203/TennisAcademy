using System;
namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Package entity
/// </summary>
public class PackageDto : BaseDto
{
    /// <summary>
    /// Title of the package
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Package price in the system's currency
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Total credits included in this package
    /// </summary>
    public int CreditCount { get; set; }

    /// <summary>
    /// Package expiration date defined by the administrator
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Number of credits currently used from this package
    /// </summary>
    public int UsedCreditCount { get; set; }
}

public class CreatePackageDto
{
    /// <summary>
    /// Title of the package
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Package price in the system's currency
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Total credits included in this package
    /// </summary>
    public int CreditCount { get; set; }

    /// <summary>
    /// Package expiration date defined by the administrator
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Number of credits currently used from this package
    /// </summary>
    public int UsedCreditCount { get; set; }
}


/// <summary>
/// DTO for package search
/// </summary>
public class PackageSearchDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? ExpiresAfter { get; set; }
    public DateTime? ExpiresBefore { get; set; }
}

/// <summary>
/// DTO for updating package capacity
/// </summary>
public class UpdateCreditsDto
{
    public int CreditCount { get; set; }
}
