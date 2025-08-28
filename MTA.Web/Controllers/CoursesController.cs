using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.DTOs.Course;
using MTA.Application.Services;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing tennis courses
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;

    public CoursesController(ICourseService courseService, IMapper mapper)
    {
        _courseService = courseService;
        _mapper = mapper;
    }

    #region Course CRUD Operations

    /// <summary>
    /// Gets all courses with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title or description</param>
    /// <param name="levelId">Filter by level ID</param>
    /// <param name="statusId">Filter by status ID</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <returns>Paginated list of courses</returns>
    /// <response code="200">Returns the list of courses</response>
    /// <response code="400">If the filter parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<CourseDto>>> GetCourses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? levelId = null,
        [FromQuery] int? statusId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        try
        {
            var result = await _courseService.GetAllAsync(page, pageSize, searchTerm, levelId, statusId, minPrice, maxPrice);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific course by ID
    /// </summary>
    /// <param name="id">The ID of the course</param>
    /// <returns>The requested course</returns>
    /// <response code="200">Returns the requested course</response>
    /// <response code="404">If the course was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CourseDto>> GetCourse(int id)
    {
        try
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound($"Course with ID {id} not found");

            return Ok(course);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new course
    /// </summary>
    /// <param name="createCourseDto">The course creation data</param>
    /// <returns>The created course</returns>
    /// <response code="201">Returns the newly created course</response>
    /// <response code="400">If the course data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [Authorize(Policy = "RolesAdminCoach")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdCourse = await _courseService.CreateAsync(createCourseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = createdCourse.Id }, createdCourse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing course
    /// </summary>
    /// <param name="id">The ID of the course to update</param>
    /// <param name="updateCourseDto">The updated course data</param>
    /// <returns>The updated course</returns>
    /// <response code="200">Returns the updated course</response>
    /// <response code="400">If the course data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="404">If the course was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "RolesAdminCoach")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CourseDto>> UpdateCourse(int id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedCourse = await _courseService.UpdateAsync(id, updateCourseDto);
            return Ok(updatedCourse);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a course
    /// </summary>
    /// <param name="id">The ID of the course to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the course was deleted successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="404">If the course was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RolesAdminCoach")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        try
        {
            var deleted = await _courseService.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Course with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Advanced Filtering and Search

    /// <summary>
    /// Gets courses with advanced filtering
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <returns>Filtered and paginated courses</returns>
    /// <response code="200">Returns the filtered courses</response>
    /// <response code="400">If the filter parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("filter")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<CourseDto>>> GetFilteredCourses([FromBody] CourseFilterDto filter)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _courseService.GetFilteredAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Searches courses by text
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Search results</returns>
    /// <response code="200">Returns the search results</response>
    /// <response code="400">If the search parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<CourseDto>>> SearchCourses(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest("Search term is required");

        try
        {
            var result = await _courseService.SearchAsync(searchTerm, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Course Recommendations and Statistics

    /// <summary>
    /// Gets recommended courses for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="count">Number of recommendations</param>
    /// <returns>Recommended courses</returns>
    /// <response code="200">Returns the recommended courses</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("recommended")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetRecommendedCourses(
        [FromQuery] int userId,
        [FromQuery] int count = 5)
    {
        try
        {
            var recommendedCourses = await _courseService.GetRecommendedAsync(userId, count);
            return Ok(recommendedCourses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets course statistics
    /// </summary>
    /// <returns>Course statistics</returns>
    /// <response code="200">Returns the course statistics</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("statistics")]
    [Authorize(Policy = "RolesAdminCoach")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CourseStatisticsDto>> GetCourseStatistics()
    {
        try
        {
            var statistics = await _courseService.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Course Management Operations

    /// <summary>
    /// Gets courses by level
    /// </summary>
    /// <param name="levelId">Level ID</param>
    /// <returns>Courses for the specified level</returns>
    /// <response code="200">Returns the courses for the level</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("level/{levelId}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByLevel(int levelId)
    {
        try
        {
            var courses = await _courseService.GetByLevelAsync(levelId);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets courses by status
    /// </summary>
    /// <param name="statusId">Status ID</param>
    /// <returns>Courses with the specified status</returns>
    /// <response code="200">Returns the courses with the status</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("status/{statusId}")]
    [Authorize(Policy = "RolesAdminCoach")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByStatus(int statusId)
    {
        try
        {
            var courses = await _courseService.GetByStatusAsync(statusId);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets popular courses
    /// </summary>
    /// <param name="count">Number of courses to return</param>
    /// <returns>Popular courses</returns>
    /// <response code="200">Returns the popular courses</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("popular")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetPopularCourses([FromQuery] int count = 10)
    {
        try
        {
            var popularCourses = await _courseService.GetPopularCoursesAsync(count);
            return Ok(popularCourses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets free courses
    /// </summary>
    /// <returns>Free courses</returns>
    /// <response code="200">Returns the free courses</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("free")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetFreeCourses()
    {
        try
        {
            var freeCourses = await _courseService.GetFreeCoursesAsync();
            return Ok(freeCourses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region Course Status and Level Management

    /// <summary>
    /// Changes the course status
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="statusId">New status ID</param>
    /// <returns>The updated course</returns>
    /// <response code="200">Returns the updated course</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="404">If the course was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/status")]
    [Authorize(Policy = "RolesAdminCoach")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CourseDto>> ChangeCourseStatus(int id, [FromBody] int statusId)
    {
        try
        {
            var updatedCourse = await _courseService.ChangeStatusAsync(id, statusId);
            return Ok(updatedCourse);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Changes the course level
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="levelId">New level ID</param>
    /// <returns>The updated course</returns>
    /// <response code="200">Returns the updated course</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="404">If the course was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/level")]
    [Authorize(Policy = "RolesAdminCoach")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CourseDto>> ChangeCourseLevel(int id, [FromBody] int levelId)
    {
        try
        {
            var updatedCourse = await _courseService.ChangeLevelAsync(id, levelId);
            return Ok(updatedCourse);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the course price
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="price">New price</param>
    /// <returns>The updated course</returns>
    /// <response code="200">Returns the updated course</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="404">If the course was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/price")]
    [Authorize(Policy = "RolesAdminCoach")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CourseDto>> UpdateCoursePrice(int id, [FromBody] decimal price)
    {
        try
        {
            var updatedCourse = await _courseService.UpdatePriceAsync(id, price);
            return Ok(updatedCourse);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion
}
