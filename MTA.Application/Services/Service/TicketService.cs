using Microsoft.EntityFrameworkCore;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services.Service;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageService _messageService;

    public TicketService(IUnitOfWork unitOfWork, IMessageService messageService)
    {
        _unitOfWork = unitOfWork;
        _messageService = messageService;
    }

    public async Task<PaginatedResult<TicketDto>> GetAllAsync(
        int page = 1, int pageSize = 10,
        string? searchTerm = null, int? statusId = null,
        int? accountId = null, int? packageId = null,
        CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Ticket>().GetQueryable()
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Account).ThenInclude(a => a.UserProfile)
            .Include(t => t.Package)
            .Include(t => t.Messages)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(t => t.Topic.Contains(searchTerm));
        if (statusId.HasValue)
            query = query.Where(t => t.StatusId == statusId.Value);
        if (accountId.HasValue)
            query = query.Where(t => t.AccountId == accountId.Value);
        if (packageId.HasValue)
            query = query.Where(t => t.PackageId == packageId.Value);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<TicketDto>
        {
            Data = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TicketDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetQueryable()
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Account).ThenInclude(a => a.UserProfile)
            .Include(t => t.Package)
            .Include(t => t.Messages)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        return ticket is null ? null : MapToDto(ticket);
    }

    public async Task<IEnumerable<TicketDto>> GetByAccountAsync(int accountId, CancellationToken ct = default)
    {
        var tickets = await _unitOfWork.Repository<Ticket>().GetQueryable()
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Account).ThenInclude(a => a.UserProfile)
            .Include(t => t.Messages)
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return tickets.Select(MapToDto);
    }

    public async Task<IEnumerable<TicketDto>> GetByStatusAsync(int statusId, CancellationToken ct = default)
    {
        var tickets = await _unitOfWork.Repository<Ticket>().GetQueryable()
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Account).ThenInclude(a => a.UserProfile)
            .Where(t => t.StatusId == statusId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return tickets.Select(MapToDto);
    }

    public async Task<IEnumerable<TicketDto>> GetByPackageAsync(int packageId, CancellationToken ct = default)
    {
        var tickets = await _unitOfWork.Repository<Ticket>().GetQueryable()
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Package)
            .Where(t => t.PackageId == packageId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return tickets.Select(MapToDto);
    }

    // RULE-3: Full ticket creation flow — credit deduction
    public async Task<TicketDto> CreateAsync(CreateTicketDto dto, CancellationToken ct = default)
    {
        // Find "Pending" status from Lookups
        var pendingStatus = await _unitOfWork.Repository<Lookup>().GetQueryable()
            .FirstOrDefaultAsync(l => l.Category == "TicketStatus" && l.Key == "Pending", ct)
            ?? throw new InvalidOperationException("Ticket status 'Pending' not found in Lookups.");

        // Load account with package history
        var account = await _unitOfWork.Repository<Account>().GetQueryable()
            .Include(a => a.PackageHistory)
                .ThenInclude(ph => ph.Package)
            .Include(a => a.UserProfile)
            .FirstOrDefaultAsync(a => a.Id == dto.AccountId, ct)
            ?? throw new InvalidOperationException("Account not found.");

        // RULE-3: earliest-expiring active package with tickets remaining
        var activePackage = account.PackageHistory
            .Where(ph => ph.ExpiredDate > DateTime.UtcNow && ph.RemainingTickets > 0)
            .OrderBy(ph => ph.ExpiredDate)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("No active package with remaining tickets found.");

        // Create ticket
        var ticket = new Ticket
        {
            Topic = dto.Topic,
            StatusId = pendingStatus.Id,
            AccountId = dto.AccountId,
            PackageId = activePackage.PackageId
        };
        await _unitOfWork.Repository<Ticket>().AddAsync(ticket, ct);

        // Decrement credit
        activePackage.RemainingTickets = Math.Max(0, activePackage.RemainingTickets - 1);
        await _unitOfWork.Repository<PackageHistory>().UpdateAsync(activePackage, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        // Reload with navigations for full DTO
        var full = await _unitOfWork.Repository<Ticket>().GetQueryable()
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Package)
            .Include(t => t.Messages)
            .Include(t => t.Account).ThenInclude(a => a.UserProfile)
            .FirstOrDefaultAsync(t => t.Id == ticket.Id, ct)
            ?? throw new InvalidOperationException("Ticket could not be loaded after creation.");

        return MapToDto(full);
    }

    public async Task<TicketDto> UpdateAsync(int id, TicketDto ticketDto, CancellationToken ct = default)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Ticket with ID {id} not found");

        ticket.StatusId = ticketDto.StatusId;
        await _unitOfWork.Repository<Ticket>().UpdateAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Ticket reload failed.");
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var messageIds = await _unitOfWork.Repository<Message>().GetQueryable()
            .Where(m => m.TicketId == id)
            .Select(m => m.Id)
            .ToListAsync(ct);

        var deleted = await _unitOfWork.Repository<Ticket>().DeleteAsync(id, ct);
        if (!deleted) return false;

        await _unitOfWork.SaveChangesAsync(ct);

        // Cascade through the ticket's messages so their media files (and the
        // physical files on disk) get cleaned up too, not just the ticket row.
        foreach (var messageId in messageIds)
        {
            await _messageService.DeleteAsync(messageId, ct);
        }

        return true;
    }

    public async Task<TicketDto> ChangeStatusAsync(int id, int statusId, CancellationToken ct = default)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Ticket with ID {id} not found");

        ticket.StatusId = statusId;
        await _unitOfWork.Repository<Ticket>().UpdateAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Ticket reload failed.");
    }

    public async Task<TicketDto> AssignToPackageAsync(int id, int packageId, CancellationToken ct = default)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Ticket with ID {id} not found");

        ticket.PackageId = packageId;
        await _unitOfWork.Repository<Ticket>().UpdateAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Ticket reload failed.");
    }

    public async Task<TicketStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        var tickets = await _unitOfWork.Repository<Ticket>().GetQueryable()
            .AsNoTracking()
            .Include(t => t.Status)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        return new TicketStatisticsDto
        {
            TotalTickets = tickets.Count,
            PendingTickets = tickets.Count(t => t.Status?.Key == "Pending"),
            OpenTickets = tickets.Count(t => t.Status?.Key == "InProgress"),
            ResolvedTickets = tickets.Count(t => t.Status?.Key == "Resolved"),
            ClosedTickets = tickets.Count(t => t.Status?.Key == "Closed"),
            TicketsThisMonth = tickets.Count(t => t.CreatedAt.Month == now.Month && t.CreatedAt.Year == now.Year),
            TicketsLastMonth = tickets.Count(t => t.CreatedAt.Month == now.AddMonths(-1).Month && t.CreatedAt.Year == now.AddMonths(-1).Year)
        };
    }

    public async Task<IEnumerable<TicketDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var tickets = await _unitOfWork.Repository<Ticket>().GetQueryable()
            .AsNoTracking()
            .Include(t => t.Status)
            .Include(t => t.Account).ThenInclude(a => a.UserProfile)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return tickets.Select(MapToDto);
    }

    private static TicketDto MapToDto(Ticket t) => new()
    {
        Id = t.Id,
        Topic = t.Topic,
        StatusId = t.StatusId,
        StatusValue = t.Status?.Value,
        AccountId = t.AccountId,
        UserFirstName = t.Account?.UserProfile?.FirstName,
        UserLastName = t.Account?.UserProfile?.LastName,
        PackageId = t.PackageId,
        PackageTitle = t.Package?.Title,
        MessageCount = t.Messages?.Count ?? 0,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };
}
