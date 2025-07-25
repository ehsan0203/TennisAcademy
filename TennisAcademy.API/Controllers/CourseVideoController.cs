using Microsoft.AspNetCore.Mvc;
using TennisAcademy.API.Extensions;
using TennisAcademy.Application.DTOs.CourseVideo;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseVideoController : ControllerBase
    {
        private readonly ICourseVideoService _service;

        public CourseVideoController(ICourseVideoService service)
        {
            _service = service;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            Guid? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                userId = User.GetUserId();
            }
            var videos = await _service.GetVideosForUserAsync(courseId, userId);
            var result = videos.Select(v => new CourseVideoDto
            {
                Id = v.Id,
                Title = v.Title,
                VideoUrl = v.VideoUrl,
                DurationMinutes = v.DurationMinutes,
                IsFreePreview = v.IsFreePreview
            });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseVideoDto dto)
        {
            var video = new CourseVideo
            {
                Id = Guid.NewGuid(),
                CourseId = dto.CourseId,
                Title = dto.Title,
                VideoUrl = dto.VideoUrl,
                DurationMinutes = dto.DurationMinutes,
                IsFreePreview = dto.IsFreePreview
            };
            await _service.AddAsync(video);
            return Ok();
        }
    }
}
