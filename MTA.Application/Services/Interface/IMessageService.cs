using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IMessageService
{
    Task<PaginatedResult<MessageDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? ticketId = null, int? senderId = null, bool? isRead = null, CancellationToken ct = default);
    Task<MessageDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<MessageDto>> GetByTicketAsync(int ticketId, CancellationToken ct = default);
    Task<IEnumerable<MessageDto>> GetBySenderAsync(int senderId, CancellationToken ct = default);
    Task<IEnumerable<MessageDto>> GetUnreadMessagesAsync(CancellationToken ct = default);
    Task<IEnumerable<MessageDto>> GetUnreadMessagesByTicketAsync(int ticketId, CancellationToken ct = default);
    Task<MessageDto> CreateAsync(CreateMessageDto messageDto, CancellationToken ct = default);
    Task<MessageDto> UpdateAsync(int id, UpdateMessageDto messageDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<MessageDto> MarkAsReadAsync(int id, CancellationToken ct = default);
    Task<MessageDto> MarkAsUnreadAsync(int id, CancellationToken ct = default);
    Task<int> MarkAllAsReadByTicketAsync(int ticketId, CancellationToken ct = default);
    Task<IEnumerable<MessageDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}
