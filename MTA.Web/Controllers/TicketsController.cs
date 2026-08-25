using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;
using System.Security.Claims;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<TicketDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<TicketDto>>> GetTickets(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? statusId = null,
        [FromQuery] int? accountId = null,
        [FromQuery] int? packageId = null,
        CancellationToken ct = default)
    {
        var result = await _ticketService.GetAllAsync(page, pageSize, searchTerm, statusId, accountId, packageId, ct);
        return CustomJsonResult<PaginatedResult<TicketDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<TicketDto>> GetTicket(int id, CancellationToken ct)
    {
        var ticket = await _ticketService.GetByIdAsync(id, ct);
        return ticket is null
            ? CustomJsonResult<TicketDto>.NotFound($"Ticket with ID {id} not found")
            : CustomJsonResult<TicketDto>.SuccessResult(ticket);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<TicketDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status401Unauthorized)]
    public async Task<CustomJsonResult<TicketDto>> CreateTicket([FromBody] CreateTicketDto ticketDto, CancellationToken ct)
    {
        // Reading the current user ID from claims is controller territory (RULE-7: primary claim is "UserId" fallback NameIdentifier)
        var claimValue = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claimValue, out var accountId))
            return CustomJsonResult<TicketDto>.Unauthorized("Account ID not found in token.");

        var dto = new CreateTicketDto { Topic = ticketDto.Topic, AccountId = accountId };
        var created = await _ticketService.CreateAsync(dto, ct);
        return CustomJsonResult<TicketDto>.Created(created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<TicketDto>> UpdateTicket(int id, [FromBody] TicketDto ticketDto, CancellationToken ct)
    {
        var updated = await _ticketService.UpdateAsync(id, ticketDto, ct);
        return CustomJsonResult<TicketDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteTicket(int id, CancellationToken ct)
    {
        var deleted = await _ticketService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Ticket with ID {id} not found");
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<TicketDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<TicketDto>>> GetTicketsByUser(int userId, CancellationToken ct)
    {
        var tickets = await _ticketService.GetByAccountAsync(userId, ct);
        return CustomJsonResult<IEnumerable<TicketDto>>.SuccessResult(tickets);
    }

    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(CustomJsonResult<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<TicketDto>> ChangeStatus(int id, [FromBody] int statusId, CancellationToken ct)
    {
        var updated = await _ticketService.ChangeStatusAsync(id, statusId, ct);
        return CustomJsonResult<TicketDto>.SuccessResult(updated);
    }
}
