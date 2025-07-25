using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TennisAcademy.Application.DTOs.Ticket;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;
using TennisAcademy.Domain.Enums;

namespace TennisAcademy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IUserScoreService _userScoreService;

        public TicketController(ITicketService ticketService, IUserScoreService userScoreService)
        {
            _ticketService = ticketService;
            _userScoreService = userScoreService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
        {
            var credit = await _userScoreService.GetUserCreditAsync(dto.UserId);
            if (credit < 1)
                return BadRequest("شما کردیت کافی برای ارسال تیکت ندارید.");

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Subject = dto.Subject,
                Description = dto.Description,
                FileUrl = dto.FileUrl,
                VoiceUrl = dto.VoiceUrl,
                UserId = dto.UserId,
                CoachId = null,
                CreatedAt = DateTime.UtcNow,
                Status = TicketStatus.Waiting
            };

            await _ticketService.AddTicketAsync(ticket);

            return Ok();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserTickets(Guid userId)
        {
            var tickets = await _ticketService.GetByUserAsync(userId);
            var result = tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                Subject = t.Subject,
                Description = t.Description,
                FileUrl = t.FileUrl,
                VoiceUrl = t.VoiceUrl,
                CoachReply = t.CoachReply,
                CreatedAt = t.CreatedAt,
                Status = t.Status.ToString()
            });

            return Ok(result);
        }

        [HttpPatch("answer")]
        public async Task<IActionResult> AnswerTicket([FromBody] AnswerTicketDto dto)
        {
            await _ticketService.AnswerTicketAsync(dto);
            return Ok();
        }

        [HttpPatch("{id}/close")]
        public async Task<IActionResult> CloseTicket(Guid id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
                return NotFound();

            await _ticketService.CloseTicketAsync(id);
            await _userScoreService.DecreaseCreditAsync(ticket.UserId, 1);

            return Ok();
        }
    }
}
