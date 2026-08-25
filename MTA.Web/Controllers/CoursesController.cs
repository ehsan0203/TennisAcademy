using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.DTOs.Course;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<CourseDto>>> GetCourses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? levelId = null,
        [FromQuery] int? statusId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        CancellationToken ct = default)
    {
        var result = await _courseService.GetAllAsync(page, pageSize, searchTerm, levelId, statusId, minPrice, maxPrice, ct);
        return CustomJsonResult<PaginatedResult<CourseDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<CourseDto>> GetCourse(int id, CancellationToken ct)
    {
        var course = await _courseService.GetByIdAsync(id, ct);
        return course is null
            ? CustomJsonResult<CourseDto>.NotFound($"Course with ID {id} not found")
            : CustomJsonResult<CourseDto>.SuccessResult(course);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<CourseDto>> CreateCourse([FromForm] CreateCourseDto createCourseDto, CancellationToken ct)
    {
        var created = await _courseService.CreateAsync(createCourseDto, ct);
        return CustomJsonResult<CourseDto>.Created(created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<CourseDto>> UpdateCourse(int id, [FromForm] UpdateCourseDto updateCourseDto, CancellationToken ct)
    {
        var updated = await _courseService.UpdateAsync(id, updateCourseDto, ct);
        return CustomJsonResult<CourseDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteCourse(int id, CancellationToken ct)
    {
        var deleted = await _courseService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Course with ID {id} not found");
    }

    [HttpPost("filter")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<CourseDto>>> GetFilteredCourses([FromBody] CourseFilterDto filter, CancellationToken ct)
    {
        var result = await _courseService.GetFilteredAsync(filter, ct);
        return CustomJsonResult<PaginatedResult<CourseDto>>.SuccessResult(result);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<CourseDto>>> SearchCourses(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _courseService.SearchAsync(searchTerm, page, pageSize, ct);
        return CustomJsonResult<PaginatedResult<CourseDto>>.SuccessResult(result);
    }
}
