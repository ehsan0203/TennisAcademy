using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IChatNotifier
{
    Task NotifyMessageCreatedAsync(int ticketId, MessageDto message, CancellationToken ct = default);
    Task NotifyMessageDeletedAsync(int ticketId, int messageId, CancellationToken ct = default);
    Task NotifyTypingAsync(int ticketId, int senderId, bool isTyping, CancellationToken ct = default);
    Task NotifyMessageReadAsync(int ticketId, int messageId, int readerId, CancellationToken ct = default);
}
