using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing messages
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
//[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    #region Message CRUD Operations

    /// <summary>
    /// Gets all messages with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for text content</param>
    /// <param name="ticketId">Filter by ticket ID</param>
    /// <param name="senderId">Filter by sender ID</param>
    /// <param name="isRead">Filter by read status</param>
    /// <returns>Paginated list of messages</returns>
    /// <response code="200">Returns the list of messages</response>
    /// <response code="400">If the filter parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? ticketId = null,
        [FromQuery] int? senderId = null,
        [FromQuery] bool? isRead = null)
    {
        try
        {
            var result = await _messageService.GetAllAsync(page, pageSize, searchTerm, ticketId, senderId, isRead);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific message by ID
    /// </summary>
    /// <param name="id">The ID of the message</param>
    /// <returns>The requested message</returns>
    /// <response code="200">Returns the requested message</response>
    /// <response code="404">If the message was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MessageDto>> GetMessage(int id)
    {
        try
        {
            var message = await _messageService.GetByIdAsync(id);
            if (message == null)
                return NotFound($"Message with ID {id} not found");

            return Ok(message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new message
    /// </summary>
    /// <param name="messageDto">The message data</param>
    /// <returns>The created message</returns>
    /// <response code="201">Returns the newly created message</response>
    /// <response code="400">If the message data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MessageDto>> CreateMessage([FromBody] MessageDto messageDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdMessage = await _messageService.CreateAsync(messageDto);
            return CreatedAtAction(nameof(GetMessage), new { id = createdMessage.Id }, createdMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing message
    /// </summary>
    /// <param name="id">The ID of the message to update</param>
    /// <param name="messageDto">The updated message data</param>
    /// <returns>The updated message</returns>
    /// <response code="200">Returns the updated message</response>
    /// <response code="400">If the message data is invalid</response>
    /// <response code="404">If the message was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MessageDto>> UpdateMessage(int id, [FromBody] MessageDto messageDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedMessage = await _messageService.UpdateAsync(id, messageDto);
            return Ok(updatedMessage);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a message
    /// </summary>
    /// <param name="id">The ID of the message to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the message was deleted successfully</response>
    /// <response code="404">If the message was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        try
        {
            var deleted = await _messageService.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Message with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Message Status Operations

    /// <summary>
    /// Marks a message as read
    /// </summary>
    /// <param name="id">The ID of the message</param>
    /// <returns>The updated message</returns>
    /// <response code="200">Returns the updated message</returns>
    /// <response code="404">If the message was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/mark-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MessageDto>> MarkAsRead(int id)
    {
        try
        {
            var updatedMessage = await _messageService.MarkAsReadAsync(id);
            return Ok(updatedMessage);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Marks a message as unread
    /// </summary>
    /// <param name="id">The ID of the message</param>
    /// <returns>The updated message</returns>
    /// <response code="200">Returns the updated message</returns>
    /// <response code="404">If the message was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/mark-unread")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MessageDto>> MarkAsUnread(int id)
    {
        try
        {
            var updatedMessage = await _messageService.MarkAsUnreadAsync(id);
            return Ok(updatedMessage);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Marks all messages in a ticket as read
    /// </summary>
    /// <param name="ticketId">The ID of the ticket</param>
    /// <returns>The number of messages marked as read</returns>
    /// <response code="200">Returns the count of messages marked as read</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("ticket/{ticketId}/mark-all-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> MarkAllAsReadByTicket(int ticketId)
    {
        try
        {
            var count = await _messageService.MarkAllAsReadByTicketAsync(ticketId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Message Queries

    /// <summary>
    /// Gets messages by ticket ID
    /// </summary>
    /// <param name="ticketId">The ID of the ticket</param>
    /// <returns>List of messages in the ticket</returns>
    /// <response code="200">Returns the list of messages</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("ticket/{ticketId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesByTicket(int ticketId)
    {
        try
        {
            var messages = await _messageService.GetByTicketAsync(ticketId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets messages by sender ID
    /// </summary>
    /// <param name="senderId">The ID of the sender</param>
    /// <returns>List of messages from the sender</returns>
    /// <response code="200">Returns the list of messages</returns>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("sender/{senderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesBySender(int senderId)
    {
        try
        {
            var messages = await _messageService.GetBySenderAsync(senderId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets all unread messages
    /// </summary>
    /// <returns>List of unread messages</returns>
    /// <response code="200">Returns the list of unread messages</returns>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("unread")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetUnreadMessages()
    {
        try
        {
            var messages = await _messageService.GetUnreadMessagesAsync();
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets unread messages by ticket ID
    /// </summary>
    /// <param name="ticketId">The ID of the ticket</param>
    /// <returns>List of messages in the ticket</returns>
    /// <response code="200">Returns the list of unread messages</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("ticket/{ticketId}/unread")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetUnreadMessagesByTicket(int ticketId)
    {
        try
        {
            var messages = await _messageService.GetUnreadMessagesByTicketAsync(ticketId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets messages by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of messages in the date range</returns>
    /// <response code="200">Returns the list of messages</response>
    /// <response code="400">If the date parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("date-range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
                return BadRequest("Start date cannot be after end date");

            var messages = await _messageService.GetByDateRangeAsync(startDate, endDate);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

}
