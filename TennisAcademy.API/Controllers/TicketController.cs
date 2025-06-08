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

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Subject = dto.Subject,
                Description = dto.Description,
                FileUrl = dto.FileUrl,
                VoiceUrl = dto.VoiceUrl,
                UserId = dto.UserId,
                CoachId = dto.CoachId,
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
    }
}
