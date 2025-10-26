using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing user course history
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
//[Authorize]
public class UserCourseHistoryController : ControllerBase
{
    private readonly IUserCourseHistoryService _userCourseHistoryService;

    public UserCourseHistoryController(IUserCourseHistoryService userCourseHistoryService)
    {
        _userCourseHistoryService = userCourseHistoryService;
    }

    #region UserCourseHistory CRUD Operations

    /// <summary>
    /// Gets all user course histories with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="accountId">Filter by account ID</param>
    /// <param name="courseId">Filter by course ID</param>
    /// <returns>Paginated list of user course histories</returns>
    /// <response code="200">Returns the list of user course histories</response>
    /// <response code="400">If the filter parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<UserCourseHistoryDetailDto>>> GetUserCourseHistories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? accountId = null,
        [FromQuery] int? courseId = null)
    {
        try
        {
            var result = await _userCourseHistoryService.GetAllAsync(page, pageSize, accountId, courseId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific user course history by ID
    /// </summary>
    /// <param name="id">The ID of the user course history</param>
    /// <returns>The requested user course history</returns>
    /// <response code="200">Returns the requested user course history</response>
    /// <response code="404">If the user course history was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserCourseHistoryDetailDto>> GetUserCourseHistory(int id)
    {
        try
        {
            var userCourseHistory = await _userCourseHistoryService.GetByIdAsync(id);
            if (userCourseHistory == null)
                return NotFound($"User course history with ID {id} not found");

            return Ok(userCourseHistory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new user course history
    /// </summary>
    /// <param name="userCourseHistoryDto">The user course history data</param>
    /// <returns>The created user course history</returns>
    /// <response code="201">Returns the newly created user course history</response>
    /// <response code="400">If the user course history data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UpdateUserCourseHistoryDto>> CreateUserCourseHistory([FromBody] CreateUserCourseHistoryDto userCourseHistoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdUserCourseHistory = await _userCourseHistoryService.CreateAsync(userCourseHistoryDto);
            return CreatedAtAction(nameof(GetUserCourseHistory), new { id = createdUserCourseHistory.Id }, createdUserCourseHistory);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing user course history
    /// </summary>
    /// <param name="id">The ID of the user course history to update</param>
    /// <param name="userCourseHistoryDto">The updated user course history data</param>
    /// <returns>The updated user course history</returns>
    /// <response code="200">Returns the updated user course history</response>
    /// <response code="400">If the user course history data is invalid</response>
    /// <response code="404">If the user course history was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UpdateUserCourseHistoryDto>> UpdateUserCourseHistory(int id, [FromBody] UpdateUserCourseHistoryDto userCourseHistoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUserCourseHistory = await _userCourseHistoryService.UpdateAsync(id, userCourseHistoryDto);
            return Ok(updatedUserCourseHistory);
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
    /// Deletes a user course history
    /// </summary>
    /// <param name="id">The ID of the user course history to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the user course history was deleted successfully</response>
    /// <response code="404">If the user course history was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUserCourseHistory(int id)
    {
        try
        {
            var deleted = await _userCourseHistoryService.DeleteAsync(id);
            if (!deleted)
                return NotFound($"User course history with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region UserCourseHistory Queries

    /// <summary>
    /// Gets user course histories by account ID
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>List of user course histories for the specified account</returns>
    /// <response code="200">Returns the list of user course histories</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("account/{accountId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserCourseHistoryDetailDto>>> GetUserCourseHistoriesByAccount(int accountId)
    {
        try
        {
            var userCourseHistories = await _userCourseHistoryService.GetByAccountAsync(accountId);
            return Ok(userCourseHistories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets user course histories by course ID
    /// </summary>
    /// <param name="courseId">The ID of the course</param>
    /// <returns>List of user course histories for the specified course</returns>
    /// <response code="200">Returns the list of user course histories</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("course/{courseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserCourseHistoryDetailDto>>> GetUserCourseHistoriesByCourse(int courseId)
    {
        try
        {
            var userCourseHistories = await _userCourseHistoryService.GetByCourseAsync(courseId);
            return Ok(userCourseHistories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    ///// <summary>
    ///// Checks if a user has purchased a specific course
    ///// </summary>
    ///// <param name="accountId">The ID of the account</param>
    ///// <param name="courseId">The ID of the course</param>
    ///// <returns>True if user has purchased the course</returns>
    ///// <response code="200">Returns the result</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpGet("check-purchase")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<bool>> CheckUserHasPurchasedCourse(
    //    [FromQuery] int accountId,
    //    [FromQuery] int courseId)
    //{
    //    try
    //    {
    //        var hasPurchased = await _userCourseHistoryService.UserHasPurchasedCourseAsync(accountId, courseId);
    //        return Ok(hasPurchased);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    #endregion

    #region UserCourseHistory Analytics and Reports

    ///// <summary>
    ///// Gets user course history statistics
    ///// </summary>
    ///// <returns>User course history statistics</returns>
    ///// <response code="200">Returns the user course history statistics</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpGet("statistics")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<UserCourseHistoryStatisticsDto>> GetUserCourseHistoryStatistics()
    //{
    //    try
    //    {
    //        var statistics = await _userCourseHistoryService.GetStatisticsAsync();
    //        return Ok(statistics);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    ///// <summary>
    ///// Gets user course histories by date range
    ///// </summary>
    ///// <param name="startDate">Start date</param>
    ///// <param name="endDate">End date</param>
    ///// <returns>List of user course histories within the date range</returns>
    ///// <response code="200">Returns the list of user course histories</response>
    ///// <response code="400">If the date parameters are invalid</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpGet("date-range")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<IEnumerable<UserCourseHistoryDto>>> GetUserCourseHistoriesByDateRange(
    //    [FromQuery] DateTime startDate,
    //    [FromQuery] DateTime endDate)
    //{
    //    try
    //    {
    //        if (startDate > endDate)
    //            return BadRequest("Start date cannot be after end date");

    //        var userCourseHistories = await _userCourseHistoryService.GetByDateRangeAsync(startDate, endDate);
    //        return Ok(userCourseHistories);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    ///// <summary>
    ///// Gets popular courses by purchase count
    ///// </summary>
    ///// <param name="count">Number of courses to return</param>
    ///// <returns>List of popular courses</returns>
    ///// <response code="200">Returns the list of popular courses</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpGet("popular-courses")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<IEnumerable<CourseDto>>> GetPopularCourses([FromQuery] int count = 10)
    //{
    //    try
    //    {
    //        if (count <= 0 || count > 100)
    //            return BadRequest("Count must be between 1 and 100");

    //        var popularCourses = await _userCourseHistoryService.GetPopularCoursesAsync(count);
    //        return Ok(popularCourses);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    ///// <summary>
    ///// Gets user learning progress
    ///// </summary>
    ///// <param name="accountId">The ID of the account</param>
    ///// <returns>User learning progress information</returns>
    ///// <response code="200">Returns the user learning progress</response>
    ///// <response code="404">If no course history found for the account</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpGet("learning-progress/{accountId}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<UserLearningProgressDto>> GetUserLearningProgress(int accountId)
    //{
    //    try
    //    {
    //        var learningProgress = await _userCourseHistoryService.GetUserLearningProgressAsync(accountId);
    //        return Ok(learningProgress);
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return NotFound(ex.Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    #endregion
}
