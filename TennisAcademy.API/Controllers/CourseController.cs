using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TennisAcademy.Application.DTOs.Course;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            var result = courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Price = c.Price,
                CoverImageUrl = c.CoverImageUrl,
                TotalDurationMinutes = c.TotalDurationMinutes,
                IsActive = c.IsActive
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            return Ok(new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CoverImageUrl = course.CoverImageUrl,
                TotalDurationMinutes = course.TotalDurationMinutes,
                Price = course.Price,
                IsActive = course.IsActive
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                CoverImageUrl = dto.CoverImageUrl,
                VideoIntroUrl = dto.VideoIntroUrl,
                TotalDurationMinutes = dto.TotalDurationMinutes,
                Price = dto.Price,
                IsActive = true
            };

            await _courseService.AddCourseAsync(course);

            return Ok();
        }
    }
}
