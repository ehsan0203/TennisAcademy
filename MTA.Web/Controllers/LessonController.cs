using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using Microsoft.Extensions.Logging;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;
    private readonly ILogger<LessonController> _logger;

    public LessonController(ILessonService lessonService, ILogger<LessonController> logger)
    {
        _lessonService = lessonService;
        _logger = logger;
    }

    #region CRUD Operations

    /// <summary>
    /// Get all lessons with pagination and filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<LessonDto>>> GetLessons(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? courseId = null,
        [FromQuery] bool? isFree = null)
    {
        try
        {
            var result = await _lessonService.GetAllAsync(page, pageSize, searchTerm, courseId, isFree);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lessons");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get lesson by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetLesson(int id)
    {
        try
        {
            var lesson = await _lessonService.GetByIdAsync(id);
            if (lesson == null)
                return NotFound("Lesson not found");

            return Ok(lesson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lesson with ID: {LessonId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get lessons by course ID
    /// </summary>
    [HttpGet("by-course/{courseId}")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByCourse(int courseId)
    {
        try
        {
            var lessons = await _lessonService.GetByCourseAsync(courseId);
            return Ok(lessons);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lessons by course ID: {CourseId}", courseId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create new lesson
    /// </summary>
    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<LessonDto>> CreateLesson([FromForm] CreateLessonDto lessonDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdLesson = await _lessonService.CreateAsync(lessonDto);
            return CreatedAtAction(nameof(GetLesson), new { id = createdLesson.Id }, createdLesson);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lesson");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update existing lesson
    /// </summary>
    [HttpPut("{id}")]
   // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LessonDto>> UpdateLesson(int id, [FromForm] UpdateLessonDto lessonDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedLesson = await _lessonService.UpdateAsync(id, lessonDto);
            if (updatedLesson == null)
                return NotFound("Lesson not found");

            return Ok(updatedLesson);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lesson with ID: {LessonId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete lesson
    /// </summary>
    [HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteLesson(int id)
    {
        try
        {
            var result = await _lessonService.DeleteAsync(id);
            if (!result)
                return NotFound("Lesson not found");

            return Ok("Lesson deleted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lesson with ID: {LessonId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion

    #region Advanced Operations

    /// <summary>
    /// Change lesson course
    /// </summary>
    [HttpPatch("{id}/course")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<LessonDto>> ChangeCourse(int id, [FromBody] int courseId)
    {
        try
        {
            var updatedLesson = await _lessonService.ChangeCourseAsync(id, courseId);
            return Ok(updatedLesson);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing course for lesson with ID: {LessonId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update lesson order
    /// </summary>
    [HttpPatch("{id}/order")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<LessonDto>> UpdateOrder(int id, [FromBody] int order)
    {
        try
        {
            var updatedLesson = await _lessonService.UpdateOrderAsync(id, order);
            return Ok(updatedLesson);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order for lesson with ID: {LessonId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion

    #region Search and Filter

    /// <summary>
    /// Search lessons by title or description
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<PaginatedResult<LessonDto>>> SearchLessons([FromBody] LessonSearchDto searchDto)
    {
        try
        {
            var result = await _lessonService.GetAllAsync(
                searchDto.Page, 
                searchDto.PageSize, 
                searchDto.SearchTerm,
                searchDto.CourseId,
                searchDto.IsFree);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching lessons");
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion
}

/// <summary>
/// DTO for lesson search
/// </summary>

