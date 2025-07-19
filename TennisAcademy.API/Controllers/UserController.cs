using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TennisAcademy.API.Extensions;
using TennisAcademy.Application.DTOs.UserScore;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Application.Interfaces.Services;
using BuildingBlocks.Response;

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
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> GetCredit()
        {
            var userId = User.GetUserId(); // از Claim
            var credit = await _userScoreService.GetUserCreditAsync(userId);
            return new CustomJsonResult<object>(new { credit });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-credit")]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> AddCredit([FromBody] AddCreditDto dto)
        {
            if (dto.Amount <= 0)
                return new CustomJsonResult<string>(null, StatusCodes.Status400BadRequest, "مقدار باید بیشتر از صفر باشد.");

            await _userScoreService.AddCreditAsync(dto.UserId, dto.Amount);
            return new CustomJsonResult<object>(new { message = "کردیت با موفقیت اضافه شد." });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("credit-history")]
        [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<object>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> GetCreditHistory()
        {
            var history = await _creditHistoryRepository.GetAllAsync();
            return new CustomJsonResult<IEnumerable<object>>(history.Select(h => new
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
