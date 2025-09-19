using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using MTA.Infrastructure.Data;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing support tickets
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TicketsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public TicketsController(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    /// <summary>
    /// Gets all support tickets with user information
    /// </summary>
    /// <returns>List of all tickets with user details</returns>
    /// <response code="200">Returns the list of tickets</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets()
    {
        try
        {
            var tickets = await _context.Tickets
                .Include(t => t.Package)
                .Include(t => t.Package.PackageHistories)
                .Include(t => t.Messages)
                .Include(t => t.Account)
                    .ThenInclude(a => a.UserProfile)
                .Include(t => t.Status)
                .ToListAsync();

            var ticketDtos = tickets.Select(t => new TicketDto
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
                MessageCount = t.Messages?.Count ?? 0
            });

            return Ok(ticketDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    /// <summary>
    /// Gets a specific ticket by ID with full details
    /// </summary>
    /// <param name="id">The ID of the ticket</param>
    /// <returns>The requested ticket with full details</returns>
    /// <response code="200">Returns the requested ticket</response>
    /// <response code="404">If the ticket was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketDto>> GetTicket(int id)
    {
        try
        {
            var ticket = await _context.Tickets
                .Include(t => t.Account)
                    .ThenInclude(a => a.UserProfile)
                .Include(t => t.Package)
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound($"Ticket with ID {id} not found");

            var ticketDto = new TicketDto
            {
                Id = ticket.Id,
                Topic = ticket.Topic,
                StatusId = ticket.StatusId,
                StatusValue = ticket.Status?.Value, 
                AccountId = ticket.AccountId,
                UserFirstName = ticket.Account.UserProfile?.FirstName,
                UserLastName = ticket.Account.UserProfile?.LastName,
                PackageId = ticket.PackageId,
                PackageTitle = ticket.Package?.Title,
                MessageCount = ticket.Messages?.Count ?? 0,
            };

            return Ok(ticketDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    /// <summary> 
    /// /// Creates a new support ticket 
    /// /// </summary> 
    /// /// <param name="ticketDto">The ticket data</param> 
    /// /// <returns>The created ticket</returns> 
    /// /// <response code="201">Returns the newly created ticket</response> 
    /// /// <response code="400">If the ticket data is invalid</response> 
    /// /// <response code="500">If there was an internal server error</response>
    /// 
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketDto ticketDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ticketDto.Topic))
            {
                return BadRequest("Topic is required");
            }

            var ticket = new Ticket
            {
                Topic = ticketDto.Topic,
                StatusId = 21, 
                AccountId = ticketDto.AccountId,
                PackageId = ticketDto.PackageId
            };

            var createdTicket = await _unitOfWork.Repository<Ticket>().AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            var ticketWithNav = await _unitOfWork.Repository<Ticket>()
                .GetQueryable()
                .Include(t => t.Status)
                .Include(t => t.Package)
                .Include(t => t.Account)
                    .ThenInclude(a => a.UserProfile)
                .FirstOrDefaultAsync(t => t.Id == createdTicket.Id);

            if (ticketWithNav == null)
                return StatusCode(500, "Ticket could not be loaded after creation.");


            // ساخت DTO امن از لحاظ null
            var resultDto = new TicketDto
            {
                Id = createdTicket.Id,
                Topic = createdTicket.Topic,
                StatusId = createdTicket.StatusId,
                StatusValue = createdTicket.Status?.Value,
                AccountId = createdTicket.AccountId,
                PackageId = createdTicket.PackageId,
                PackageTitle = createdTicket.Package?.Title,
                UserFirstName = createdTicket.Account?.UserProfile?.FirstName,
                UserLastName = createdTicket.Account?.UserProfile?.LastName,
                MessageCount = 0,
                CreatedAt = createdTicket.CreatedAt,
                UpdatedAt = createdTicket.UpdatedAt
            };

            return CreatedAtAction(nameof(GetTicket), new { id = resultDto.Id }, resultDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    /// <summary>
    /// Updates an existing ticket status
    /// </summary>
    /// <param name="id">The ID of the ticket to update</param>
    /// <param name="ticketDto">The updated ticket data</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the ticket was updated successfully</response>
    /// <response code="400">If the ticket data is invalid</response>
    /// <response code="404">If the ticket was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTicket(int id, [FromBody] TicketDto ticketDto)
    {
        try
        {
            // ????? ?????????? ???? StatusId
            if (ticketDto.StatusId <= 0)
            {
                return BadRequest("Valid StatusId is required");
            }

            var existingTicket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id);
            if (existingTicket == null)
            {
                return NotFound($"Ticket with ID {id} not found");
            }

            existingTicket.StatusId = ticketDto.StatusId;
            existingTicket.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Ticket>().UpdateAsync(existingTicket);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    /// <summary>
    /// Deletes a ticket
    /// </summary>
    /// <param name="id">The ID of the ticket to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the ticket was deleted successfully</response>
    /// <response code="404">If the ticket was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        try
        {
            var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound($"Ticket with ID {id} not found");
            }

            var deleted = await _unitOfWork.Repository<Ticket>().DeleteAsync(id);
            if (deleted)
            {
                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }

            return StatusCode(500, "Failed to delete ticket");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets tickets by user ID
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>List of tickets for the specified user</returns>
    /// <response code="200">Returns the list of user tickets</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetTicketsByUser(int userId)
    {
        try
        {
            var tickets = await _context.Tickets
                .Include(t => t.Account)
                    .ThenInclude(a => a.UserProfile) 
                .Include(t => t.Messages)
                .Where(t => t.AccountId == userId)
                .ToListAsync();

            var ticketDtos = tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                Topic = t.Topic,
                StatusId = t.StatusId,
                StatusValue = t.Status?.Value, 
                UserFirstName = t.Account.UserProfile?.FirstName,
                UserLastName = t.Account.UserProfile?.LastName,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                MessageCount = t.Messages?.Count ?? 0
            });

            return Ok(ticketDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
