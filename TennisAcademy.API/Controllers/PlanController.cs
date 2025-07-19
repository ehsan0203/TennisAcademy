using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Response;
using System.Net;
using TennisAcademy.Application.DTOs.Plan;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<PlanDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _planService.GetAllPlansAsync();
            var result = plans.Select(p => new PlanDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price
            });

            return new CustomJsonResult<IEnumerable<PlanDto>>(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create([FromBody] CreatePlanDto dto)
        {
            var plan = new Plan
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price
            };

            await _planService.AddPlanAsync(plan);

            return new CustomJsonResult<string>(null);
        }
    }
}
