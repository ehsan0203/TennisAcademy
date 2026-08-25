using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MTA.Web.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public Task JoinTicket(int ticketId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, TicketGroup(ticketId));

    public Task LeaveTicket(int ticketId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, TicketGroup(ticketId));

    public Task Typing(int ticketId, bool isTyping)
    {
        var senderId = ParseUserId();
        return Clients.OthersInGroup(TicketGroup(ticketId))
            .SendAsync("Typing", new { ticketId, senderId, isTyping });
    }

    private int ParseUserId()
    {
        var raw = Context.User?.FindFirst("UserId")?.Value
                  ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(raw, out var id) ? id : 0;
    }

    internal static string TicketGroup(int ticketId) => $"ticket-{ticketId}";
}
