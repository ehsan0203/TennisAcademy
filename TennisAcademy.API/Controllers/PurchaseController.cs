using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TennisAcademy.API.Extensions;
using TennisAcademy.Application.DTOs.Purchase;
using TennisAcademy.Application.Interfaces.Services;
using BuildingBlocks.Response;

namespace TennisAcademy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<PurchaseResultDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> GetAllPurchases()
        {
            var list = await _purchaseService.GetAllAsync();
            return new CustomJsonResult<IEnumerable<PurchaseResultDto>>(list);
        }
        [Authorize(Roles = "User,Coach")]
        [HttpPost]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
        {
            if (dto.CourseId == null && dto.PlanId == null)
                return new CustomJsonResult<string>(null, StatusCodes.Status400BadRequest, "Either CourseId or PlanId is required.");

            var userId = User.GetUserId(); // 👈 از Claimها
            await _purchaseService.AddPurchaseAsync(userId, dto);

            return new CustomJsonResult<object>(new { message = "Purchase successful." });
        }



        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<PurchaseResultDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> GetByUser(Guid userId, [FromQuery] string? type)
        {
            var result = await _purchaseService.GetUserPurchasesAsync(userId, type);
            return new CustomJsonResult<IEnumerable<PurchaseResultDto>>(result);
        }


    }

}
