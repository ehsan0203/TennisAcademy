namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for PackageHistory entity
/// </summary>
public class PackageHistoryDto : BaseDto
{
    /// <summary>
    /// Date when the package expires
    /// </summary>
    public DateTime ExpiredDate { get; set; }
    
    /// <summary>
    /// Number of remaining tickets
    /// </summary>
    public int RemainingTickets { get; set; }
    
    /// <summary>
    /// Deprecated: message limits removed, always 0
    /// </summary>
    public int RemainingMessages { get; set; }
    
    /// <summary>
    /// Package ID
    /// </summary>
    public int PackageId { get; set; }
    
    /// <summary>
    /// Package title
    /// </summary>
    public string? PackageTitle { get; set; }
    
    /// <summary>
    /// Price that the user paid when purchasing the package
    /// </summary>
    public decimal PackagePrice { get; set; }
    
    /// <summary>
    /// Total number of tickets in the package
    /// </summary>
    public int TotalTickets { get; set; }
    
    /// <summary>
    /// Deprecated: message limits removed, always 0
    /// </summary>
    public int TotalMessages { get; set; }
    
    /// <summary>
    /// Account ID
    /// </summary>
    public int AccountId { get; set; }
    
    /// <summary>
    /// User's first name
    /// </summary>
    public string? UserFirstName { get; set; }
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string? UserLastName { get; set; }
    
    /// <summary>
    /// User's email
    /// </summary>
    public string? UserEmail { get; set; }
    
    /// <summary>
    /// Whether the package is expired
    /// </summary>
    public bool IsExpired { get; set; }
}


public class CreatePackageHistoryDto
{
    /// <summary>
    /// Package ID
    /// </summary>
    public int PackageId { get; set; }

    /// <summary>
    /// Account ID
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Whether the package is expired
    /// </summary>
    public bool IsExpired { get; set; }
}

