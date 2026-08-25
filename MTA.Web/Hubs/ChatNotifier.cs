using Microsoft.AspNetCore.SignalR;
using MTA.Application.DTOs;
using MTA.Application.Services;

namespace MTA.Web.Hubs;

public class ChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub> _hub;

    public ChatNotifier(IHubContext<ChatHub> hub)
    {
        _hub = hub;
    }

    public Task NotifyMessageCreatedAsync(int ticketId, MessageDto message, CancellationToken ct = default) =>
        _hub.Clients.Group(ChatHub.TicketGroup(ticketId)).SendAsync("MessageCreated", message, ct);

    public Task NotifyMessageDeletedAsync(int ticketId, int messageId, CancellationToken ct = default) =>
        _hub.Clients.Group(ChatHub.TicketGroup(ticketId)).SendAsync("MessageDeleted", new { ticketId, messageId }, ct);

    public Task NotifyTypingAsync(int ticketId, int senderId, bool isTyping, CancellationToken ct = default) =>
        _hub.Clients.Group(ChatHub.TicketGroup(ticketId)).SendAsync("Typing", new { ticketId, senderId, isTyping }, ct);

    public Task NotifyMessageReadAsync(int ticketId, int messageId, int readerId, CancellationToken ct = default) =>
        _hub.Clients.Group(ChatHub.TicketGroup(ticketId)).SendAsync("MessageRead", new { ticketId, messageId, readerId }, ct);
}
