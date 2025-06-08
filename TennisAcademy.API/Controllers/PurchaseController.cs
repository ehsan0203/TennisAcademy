using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TennisAcademy.API.Extensions;
using TennisAcademy.Application.DTOs.Purchase;
using TennisAcademy.Application.Interfaces.Services;

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
        public async Task<IActionResult> GetAllPurchases()
        {
            var list = await _purchaseService.GetAllAsync();
            return Ok(list);
        }
        [Authorize(Roles = "User,Coach")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
        {
            if (dto.CourseId == null && dto.PlanId == null)
                return BadRequest("Either CourseId or PlanId is required.");

            var userId = User.GetUserId(); // 👈 از Claimها
            await _purchaseService.AddPurchaseAsync(userId, dto);

            return Ok(new { message = "Purchase successful." });
        }



        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId, [FromQuery] string? type)
        {
            var result = await _purchaseService.GetUserPurchasesAsync(userId, type);
            return Ok(result);
        }


    }

}
