using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<LessonDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<LessonDto>>> GetLessons(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? courseId = null,
        [FromQuery] bool? isFree = null,
        CancellationToken ct = default)
    {
        var result = await _lessonService.GetAllAsync(page, pageSize, searchTerm, courseId, isFree, ct);
        return CustomJsonResult<PaginatedResult<LessonDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<LessonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<LessonDto>> GetLesson(int id, CancellationToken ct)
    {
        var lesson = await _lessonService.GetByIdAsync(id, ct);
        return lesson is null
            ? CustomJsonResult<LessonDto>.NotFound("Lesson not found")
            : CustomJsonResult<LessonDto>.SuccessResult(lesson);
    }

    [HttpGet("by-course/{courseId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<LessonDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<LessonDto>>> GetLessonsByCourse(int courseId, CancellationToken ct)
    {
        var lessons = await _lessonService.GetByCourseAsync(courseId, ct);
        return CustomJsonResult<IEnumerable<LessonDto>>.SuccessResult(lessons);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CustomJsonResult<LessonDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<LessonDto>> CreateLesson([FromForm] CreateLessonDto lessonDto, CancellationToken ct)
    {
        var created = await _lessonService.CreateAsync(lessonDto, ct);
        return CustomJsonResult<LessonDto>.Created(created);
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(CustomJsonResult<LessonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<LessonDto>> UpdateLesson(int id, [FromForm] UpdateLessonDto lessonDto, CancellationToken ct)
    {
        var updated = await _lessonService.UpdateAsync(id, lessonDto, ct);
        return updated is null
            ? CustomJsonResult<LessonDto>.NotFound("Lesson not found")
            : CustomJsonResult<LessonDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteLesson(int id, CancellationToken ct)
    {
        var deleted = await _lessonService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound("Lesson not found");
    }

    [HttpPatch("{id}/course")]
    [Authorize]
    [ProducesResponseType(typeof(CustomJsonResult<LessonDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<LessonDto>> ChangeCourse(int id, [FromBody] int courseId, CancellationToken ct)
    {
        var updated = await _lessonService.ChangeCourseAsync(id, courseId, ct);
        return CustomJsonResult<LessonDto>.SuccessResult(updated);
    }

    [HttpPatch("{id}/order")]
    [Authorize]
    [ProducesResponseType(typeof(CustomJsonResult<LessonDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<LessonDto>> UpdateOrder(int id, [FromBody] int order, CancellationToken ct)
    {
        var updated = await _lessonService.UpdateOrderAsync(id, order, ct);
        return CustomJsonResult<LessonDto>.SuccessResult(updated);
    }

    [HttpPost("search")]
    [Authorize]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<LessonDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<LessonDto>>> SearchLessons([FromBody] LessonSearchDto searchDto, CancellationToken ct)
    {
        var result = await _lessonService.GetAllAsync(
            searchDto.Page, searchDto.PageSize, searchDto.SearchTerm, searchDto.CourseId, searchDto.IsFree, ct);
        return CustomJsonResult<PaginatedResult<LessonDto>>.SuccessResult(result);
    }
}
