using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface ITicketService
{
    Task<PaginatedResult<TicketDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? statusId = null, int? accountId = null, int? packageId = null, CancellationToken ct = default);
    Task<TicketDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<TicketDto>> GetByAccountAsync(int accountId, CancellationToken ct = default);
    Task<IEnumerable<TicketDto>> GetByStatusAsync(int statusId, CancellationToken ct = default);
    Task<IEnumerable<TicketDto>> GetByPackageAsync(int packageId, CancellationToken ct = default);
    Task<TicketDto> CreateAsync(CreateTicketDto dto, CancellationToken ct = default);
    Task<TicketDto> UpdateAsync(int id, TicketDto ticketDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<TicketDto> ChangeStatusAsync(int id, int statusId, CancellationToken ct = default);
    Task<TicketDto> AssignToPackageAsync(int id, int packageId, CancellationToken ct = default);
    Task<TicketStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    Task<IEnumerable<TicketDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}

public class TicketStatisticsDto
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int ClosedTickets { get; set; }
    public int PendingTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public double AverageResolutionTime { get; set; }
    public int TicketsThisMonth { get; set; }
    public int TicketsLastMonth { get; set; }
}
