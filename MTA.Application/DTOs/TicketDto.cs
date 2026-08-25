namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Ticket entity
/// </summary>
public class TicketDto : BaseDto
{
    /// <summary>
    /// Topic or subject of the ticket
    /// </summary>
    public required string Topic { get; set; }
    
    /// <summary>
    /// Status ID for this ticket
    /// </summary>
    public int StatusId { get; set; }
    
    /// <summary>
    /// Status value
    /// </summary>
    public string? StatusValue { get; set; }
    
    /// <summary>
    /// Account ID who created this ticket
    /// </summary>
    public int AccountId { get; set; }
    
    /// <summary>
    /// User's first name who created the ticket
    /// </summary>
    public string? UserFirstName { get; set; }
    
    /// <summary>
    /// User's last name who created the ticket
    /// </summary>
    public string? UserLastName { get; set; }
    
    /// <summary>
    /// Package ID for this ticket
    /// </summary>
    public int PackageId { get; set; }
    
    /// <summary>
    /// Package title
    /// </summary>
    public string? PackageTitle { get; set; }
    
    /// <summary>
    /// Number of messages in this ticket
    /// </summary>
    public int MessageCount { get; set; }
}

public class CreateTicketDto
{
    public required string Topic { get; set; }
    public int AccountId { get; set; }
}

