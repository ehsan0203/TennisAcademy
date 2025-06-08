using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TennisAcademy.Application.DTOs.CoachMedia;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoachMediaController : ControllerBase
    {
        private readonly ICoachMediaService _service;

        public CoachMediaController(ICoachMediaService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCoachMediaDto dto)
        {
            var media = new CoachMedia
            {
                Id = Guid.NewGuid(),
                CoachId = dto.CoachId,
                Title = dto.Title,
                FileUrl = dto.FileUrl,
                Type = dto.Type
            };

            await _service.AddAsync(media);
            return Ok();
        }

        [HttpGet("{coachId}")]
        public async Task<IActionResult> GetByCoach(Guid coachId)
        {
            var list = await _service.GetByCoachAsync(coachId);
            var result = list.Select(m => new CoachMediaDto
            {
                Id = m.Id,
                Title = m.Title,
                FileUrl = m.FileUrl,
                Type = m.Type.ToString()
            });

            return Ok(result);
        }
    }

}
