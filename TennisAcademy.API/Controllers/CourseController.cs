using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Response;
using System.Net;
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
        [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<CourseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
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
                IsActive = c.IsActive
            });

            return new CustomJsonResult<IEnumerable<CourseDto>>(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomJsonResult<CourseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            return new CustomJsonResult<CourseDto>(new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CoverImageUrl = course.CoverImageUrl,
                Price = course.Price,
                IsActive = course.IsActive
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                CoverImageUrl = dto.CoverImageUrl,
                VideoIntroUrl = dto.VideoIntroUrl,
                Price = dto.Price,
                IsActive = true
            };

            await _courseService.AddCourseAsync(course);

            return new CustomJsonResult<string>(null);
        }
    }
}
