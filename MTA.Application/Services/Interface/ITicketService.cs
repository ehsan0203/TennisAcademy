using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Ticket operations
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Get all tickets with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for topic</param>
    /// <param name="statusId">Filter by status ID</param>
    /// <param name="accountId">Filter by account ID</param>
    /// <param name="packageId">Filter by package ID</param>
    /// <returns>Paginated list of tickets</returns>
    Task<PaginatedResult<TicketDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? statusId = null, int? accountId = null, int? packageId = null);
    
    /// <summary>
    /// Get ticket by ID
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <returns>Ticket details</returns>
    Task<TicketDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get tickets by account ID
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <returns>List of tickets</returns>
    Task<IEnumerable<TicketDto>> GetByAccountAsync(int accountId);
    
    /// <summary>
    /// Get tickets by status ID
    /// </summary>
    /// <param name="statusId">Status ID</param>
    /// <returns>List of tickets</returns>
    Task<IEnumerable<TicketDto>> GetByStatusAsync(int statusId);
    
    /// <summary>
    /// Get tickets by package ID
    /// </summary>
    /// <param name="packageId">Package ID</param>
    /// <returns>List of tickets</returns>
    Task<IEnumerable<TicketDto>> GetByPackageAsync(int packageId);
    
    /// <summary>
    /// Create new ticket
    /// </summary>
    /// <param name="ticketDto">Ticket data</param>
    /// <returns>Created ticket</returns>
    Task<TicketDto> CreateAsync(TicketDto ticketDto);
    
    /// <summary>
    /// Update existing ticket
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="ticketDto">Updated ticket data</param>
    /// <returns>Updated ticket</returns>
    Task<TicketDto> UpdateAsync(int id, TicketDto ticketDto);
    
    /// <summary>
    /// Delete ticket
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Change ticket status
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="statusId">New status ID</param>
    /// <returns>Updated ticket</returns>
    Task<TicketDto> ChangeStatusAsync(int id, int statusId);
    
    /// <summary>
    /// Assign ticket to package
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="packageId">Package ID</param>
    /// <returns>Updated ticket</returns>
    Task<TicketDto> AssignToPackageAsync(int id, int packageId);
    
    /// <summary>
    /// Get ticket statistics
    /// </summary>
    /// <returns>Ticket statistics</returns>
    Task<TicketStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get tickets by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of tickets</returns>
    Task<IEnumerable<TicketDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Ticket statistics DTO
/// </summary>
public class TicketStatisticsDto
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int ClosedTickets { get; set; }
    public int PendingTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public double AverageResolutionTime { get; set; } // in hours
    public int TicketsThisMonth { get; set; }
    public int TicketsLastMonth { get; set; }
}
