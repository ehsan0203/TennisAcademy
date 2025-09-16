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
    /// Number of tickets included in this package
    /// </summary>
    public int TicketCount { get; set; }
    
    /// <summary>
    /// Number of messages included in this package
    /// </summary>
    public int MessageCount { get; set; }
    
    /// <summary>
    /// Duration value of the package
    /// </summary>
    public int Duration { get; set; }
    
    /// <summary>
    /// Duration unit ID for this package
    /// </summary>
    public int DurationUnitId { get; set; }
    
    /// <summary>
    /// Duration unit value (e.g., Days, Months, Years)
    /// </summary>
    public string? DurationUnitValue { get; set; }
    
    /// <summary>
    /// Number of tickets currently used from this package
    /// </summary>
    public int UsedTicketCount { get; set; }
    
    /// <summary>
    /// Number of messages currently used from this package
    /// </summary>
    public int UsedMessageCount { get; set; }
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
    public int? DurationUnitId { get; set; }
}

/// <summary>
/// DTO for updating package capacity
/// </summary>
public class UpdateCapacityDto
{
    public int TicketCount { get; set; }
    public int MessageCount { get; set; }
}

/// <summary>
/// DTO for updating package duration
/// </summary>
public class UpdateDurationDto
{
    public int Duration { get; set; }
    public int DurationUnitId { get; set; }
}
