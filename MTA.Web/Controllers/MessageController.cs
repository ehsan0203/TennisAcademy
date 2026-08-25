using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<MessageDto>> GetMessage(int id, CancellationToken ct)
    {
        var message = await _messageService.GetByIdAsync(id, ct);
        return message is null
            ? CustomJsonResult<MessageDto>.NotFound($"Message with ID {id} not found")
            : CustomJsonResult<MessageDto>.SuccessResult(message);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<MessageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<MessageDto>> CreateMessage([FromForm] CreateMessageDto messageDto, CancellationToken ct)
    {
        var created = await _messageService.CreateAsync(messageDto, ct);
        return CustomJsonResult<MessageDto>.Created(created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<MessageDto>> UpdateMessage(int id, [FromForm] UpdateMessageDto messageDto, CancellationToken ct)
    {
        var updated = await _messageService.UpdateAsync(id, messageDto, ct);
        return CustomJsonResult<MessageDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteMessage(int id, CancellationToken ct)
    {
        var deleted = await _messageService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Message with ID {id} not found");
    }

    [HttpPatch("{id}/mark-read")]
    [ProducesResponseType(typeof(CustomJsonResult<MessageDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<MessageDto>> MarkAsRead(int id, CancellationToken ct)
    {
        var updated = await _messageService.MarkAsReadAsync(id, ct);
        return CustomJsonResult<MessageDto>.SuccessResult(updated);
    }

    [HttpPatch("{id}/mark-unread")]
    [ProducesResponseType(typeof(CustomJsonResult<MessageDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<MessageDto>> MarkAsUnread(int id, CancellationToken ct)
    {
        var updated = await _messageService.MarkAsUnreadAsync(id, ct);
        return CustomJsonResult<MessageDto>.SuccessResult(updated);
    }

    [HttpPatch("ticket/{ticketId}/mark-all-read")]
    [ProducesResponseType(typeof(CustomJsonResult<int>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<int>> MarkAllAsReadByTicket(int ticketId, CancellationToken ct)
    {
        var count = await _messageService.MarkAllAsReadByTicketAsync(ticketId, ct);
        return CustomJsonResult<int>.SuccessResult(count);
    }

    [HttpGet("ticket/{ticketId}")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<MessageDto>>> GetMessagesByTicket(int ticketId, CancellationToken ct)
    {
        var messages = await _messageService.GetByTicketAsync(ticketId, ct);
        return CustomJsonResult<IEnumerable<MessageDto>>.SuccessResult(messages);
    }

    [HttpGet("ticket/{ticketId}/unread")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<MessageDto>>> GetUnreadMessagesByTicket(int ticketId, CancellationToken ct)
    {
        var messages = await _messageService.GetUnreadMessagesByTicketAsync(ticketId, ct);
        return CustomJsonResult<IEnumerable<MessageDto>>.SuccessResult(messages);
    }
}
