using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TennisAcademy.API.Extensions;
using TennisAcademy.Application.DTOs.UserScore;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Application.Interfaces.Services;

namespace TennisAcademy.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserScoreService _userScoreService;
        private readonly ICreditHistoryRepository _creditHistoryRepository;

        public UserController(IUserScoreService userScoreService, ICreditHistoryRepository creditHistoryRepository)
        {
            _userScoreService = userScoreService;
            _creditHistoryRepository = creditHistoryRepository;
        }

        [HttpGet("credit")]
        public async Task<IActionResult> GetCredit()
        {
            var userId = User.GetUserId(); // از Claim
            var credit = await _userScoreService.GetUserCreditAsync(userId);
            return Ok(new { credit });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-credit")]
        public async Task<IActionResult> AddCredit([FromBody] AddCreditDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest("مقدار باید بیشتر از صفر باشد.");

            await _userScoreService.AddCreditAsync(dto.UserId, dto.Amount);
            return Ok(new { message = "کردیت با موفقیت اضافه شد." });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("credit-history")]
        public async Task<IActionResult> GetCreditHistory()
        {
            var history = await _creditHistoryRepository.GetAllAsync();
            return Ok(history.Select(h => new
            {
                h.UserId,
                User = $"{h.User.FirstName} {h.User.LastName}",
                h.Amount,
                h.Description,
                h.CreatedAt
            }));
        }


    }
}
