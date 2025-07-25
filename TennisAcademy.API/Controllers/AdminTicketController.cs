using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TennisAcademy.Application.DTOs.Ticket;
using TennisAcademy.Application.Interfaces.Services;

namespace TennisAcademy.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/tickets")]
    public class AdminTicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IUserScoreService _userScoreService;

        public AdminTicketController(ITicketService ticketService, IUserScoreService userScoreService)
        {
            _ticketService = ticketService;
            _userScoreService = userScoreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AdminTicketFilterDto filter)
        {
            var tickets = await _ticketService.GetAllAsync(filter);
            var result = tickets.Select(t => new AdminTicketDto
            {
                Id = t.Id,
                UserId = t.UserId,
                UserName = $"{t.User.FirstName} {t.User.LastName}",
                UserEmail = t.User.Email,
                Subject = t.Subject,
                CreatedAt = t.CreatedAt,
                LastMessageAt = t.AnsweredAt ?? t.CreatedAt,
                Status = t.Status,
                CoachName = t.Coach != null ? $"{t.Coach.FirstName} {t.Coach.LastName}" : null,
                FileUrl = t.FileUrl,
                VoiceUrl = t.VoiceUrl,
                CoachReply = t.CoachReply,
                CoachReplyVoiceUrl = t.CoachReplyVoiceUrl,
                CoachReplyVideoUrl = t.CoachReplyVideoUrl
            });

            return Ok(result);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _ticketService.GetStatisticsAsync();
            return Ok(stats);
        }

        [HttpPatch("{id}/close")]
        public async Task<IActionResult> Close(Guid id)
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
