using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net;
using TennisAcademy.Application.DTOs.Ticket;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;
using TennisAcademy.Domain.Enums;
using BuildingBlocks.Response;

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
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
        {
            var credit = await _userScoreService.GetUserCreditAsync(dto.UserId);
            if (credit < 1)
                return new CustomJsonResult<string>(null, StatusCodes.Status400BadRequest, "شما کردیت کافی برای ارسال تیکت ندارید.");

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
            await _userScoreService.DecreaseCreditAsync(dto.UserId, 1); // کاهش کردیت بعد از ارسال

            return new CustomJsonResult<string>(null);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<TicketDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
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

            return new CustomJsonResult<IEnumerable<TicketDto>>(result);
        }

        [HttpPatch("answer")]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> AnswerTicket([FromBody] AnswerTicketDto dto)
        {
            await _ticketService.AnswerTicketAsync(dto);


            return new CustomJsonResult<string>(null);
        }
    }
}
