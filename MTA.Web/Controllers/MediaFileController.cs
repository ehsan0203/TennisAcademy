using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing media files
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class MediaFileController : ControllerBase
{
    private readonly IMediaFileService _mediaFileService;

    public MediaFileController(IMediaFileService mediaFileService)
    {
        _mediaFileService = mediaFileService;
    }

    #region MediaFile CRUD Operations

    /// <summary>
    /// Gets all media files with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title</param>
    /// <param name="typeId">Filter by type ID</param>
    /// <param name="lessonId">Filter by lesson ID</param>
    /// <param name="messageId">Filter by message ID</param>
    /// <returns>Paginated list of media files</returns>
    /// <response code="200">Returns the list of media files</response>
    /// <response code="400">If the filter parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<MediaFileDto>>> GetMediaFiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? typeId = null,
        [FromQuery] int? lessonId = null,
        [FromQuery] int? messageId = null)
    {
        try
        {
            var result = await _mediaFileService.GetAllAsync(page, pageSize, searchTerm, typeId, lessonId, messageId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific media file by ID
    /// </summary>
    /// <param name="id">The ID of the media file</param>
    /// <returns>The requested media file</returns>
    /// <response code="200">Returns the requested media file</response>
    /// <response code="404">If the media file was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MediaFileDto>> GetMediaFile(int id)
    {
        try
        {
            var mediaFile = await _mediaFileService.GetByIdAsync(id);
            if (mediaFile == null)
                return NotFound($"Media file with ID {id} not found");

            return Ok(mediaFile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new media file
    /// </summary>
    /// <param name="mediaFileDto">The media file data</param>
    /// <returns>The created media file</returns>
    /// <response code="201">Returns the newly created media file</response>
    /// <response code="400">If the media file data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MediaFileDto>> CreateMediaFile([FromBody] MediaFileDto mediaFileDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdMediaFile = await _mediaFileService.CreateAsync(mediaFileDto);
            return CreatedAtAction(nameof(GetMediaFile), new { id = createdMediaFile.Id }, createdMediaFile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing media file
    /// </summary>
    /// <param name="id">The ID of the media file to update</param>
    /// <param name="mediaFileDto">The updated media file data</param>
    /// <returns>The updated media file</returns>
    /// <response code="200">Returns the updated media file</response>
    /// <response code="400">If the media file data is invalid</response>
    /// <response code="404">If the media file was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MediaFileDto>> UpdateMediaFile(int id, [FromBody] MediaFileDto mediaFileDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedMediaFile = await _mediaFileService.UpdateAsync(id, mediaFileDto);
            return Ok(updatedMediaFile);
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
    /// Deletes a media file
    /// </summary>
    /// <param name="id">The ID of the media file to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the media file was deleted successfully</response>
    /// <response code="404">If the media file was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMediaFile(int id)
    {
        try
        {
            var deleted = await _mediaFileService.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Media file with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region MediaFile Update Operations

    /// <summary>
    /// Updates the type of a media file
    /// </summary>
    /// <param name="id">The ID of the media file</param>
    /// <param name="typeId">The new type ID</param>
    /// <returns>The updated media file</returns>
    /// <response code="200">Returns the updated media file</response>
    /// <response code="404">If the media file was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MediaFileDto>> UpdateMediaFileType(int id, [FromBody] int typeId)
    {
        try
        {
            var updatedMediaFile = await _mediaFileService.UpdateTypeAsync(id, typeId);
            return Ok(updatedMediaFile);
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
    /// Updates the URL of a media file
    /// </summary>
    /// <param name="id">The ID of the media file</param>
    /// <param name="url">The new URL</param>
    /// <returns>The updated media file</returns>
    /// <response code="200">Returns the updated media file</response>
    /// <response code="404">If the media file was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/url")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MediaFileDto>> UpdateMediaFileUrl(int id, [FromBody] string url)
    {
        try
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest("URL cannot be empty");

            var updatedMediaFile = await _mediaFileService.UpdateUrlAsync(id, url);
            return Ok(updatedMediaFile);
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

    #region MediaFile Queries

    /// <summary>
    /// Gets media files by type ID
    /// </summary>
    /// <param name="typeId">The ID of the type</param>
    /// <returns>List of media files of the specified type</returns>
    /// <response code="200">Returns the list of media files</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("type/{typeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetMediaFilesByType(int typeId)
    {
        try
        {
            var mediaFiles = await _mediaFileService.GetByTypeAsync(typeId);
            return Ok(mediaFiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets media files by lesson ID
    /// </summary>
    /// <param name="lessonId">The ID of the lesson</param>
    /// <returns>List of media files in the lesson</returns>
    /// <response code="200">Returns the list of media files</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("lesson/{lessonId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetMediaFilesByLesson(int lessonId)
    {
        try
        {
            var mediaFiles = await _mediaFileService.GetByLessonAsync(lessonId);
            return Ok(mediaFiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets media files by message ID
    /// </summary>
    /// <param name="messageId">The ID of the message</param>
    /// <returns>List of media files in the message</returns>
    /// <response code="200">Returns the list of media files</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("message/{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetMediaFilesByMessage(int messageId)
    {
        try
        {
            var mediaFiles = await _mediaFileService.GetByMessageAsync(messageId);
            return Ok(mediaFiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets media files by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of media files in the date range</returns>
    /// <response code="200">Returns the list of media files</response>
    /// <response code="400">If the date parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("date-range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetMediaFilesByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
                return BadRequest("Start date cannot be after end date");

            var mediaFiles = await _mediaFileService.GetByDateRangeAsync(startDate, endDate);
            return Ok(mediaFiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets media files by file size range
    /// </summary>
    /// <param name="minSize">Minimum file size in bytes</param>
    /// <param name="maxSize">Maximum file size in bytes</param>
    /// <returns>List of media files in the size range</returns>
    /// <response code="200">Returns the list of media files</response>
    /// <response code="400">If the size parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("size-range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MediaFileDto>>> GetMediaFilesBySizeRange(
        [FromQuery] long minSize,
        [FromQuery] long maxSize)
    {
        try
        {
            if (minSize < 0 || maxSize < 0 || minSize > maxSize)
                return BadRequest("Invalid size range parameters");

            var mediaFiles = await _mediaFileService.GetByFileSizeRangeAsync(minSize, maxSize);
            return Ok(mediaFiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion
}
