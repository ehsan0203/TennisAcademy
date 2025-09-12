using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Application.Services;

namespace MTA.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class MediaFileController : ControllerBase
    {
        private readonly IMediaFileService _mediaFileService;
        private readonly ILogger<MediaFileController> _logger;
        private readonly IWebHostEnvironment _env;

        public MediaFileController(IMediaFileService mediaFileService, ILogger<MediaFileController> logger, IWebHostEnvironment env)
        {
            _mediaFileService = mediaFileService;
            _logger = logger;
            _env = env;
        }

        #region Create / Upload
        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] MediaFileUploadDto mediaFileDto)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required");

            try
            {
                var created = await _mediaFileService.CreateAsync(file, mediaFileDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading media file");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region Get All / Search / Filter
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            int? typeId = null,
            int? lessonId = null,
            int? messageId = null)
        {
            var result = await _mediaFileService.GetAllAsync(page, pageSize, searchTerm, typeId, lessonId, messageId);
            return Ok(result);
        }
        #endregion

        #region Get By ID
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var media = await _mediaFileService.GetByIdAsync(id);
            if (media == null)
                return NotFound();

            return Ok(media);
        }
        #endregion

        #region Update / Replace File
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromForm] IFormFile? file, [FromForm] MediaFileUploadDto mediaFileDto)
        {
            if (string.IsNullOrWhiteSpace(mediaFileDto.Title))
                return BadRequest("Title is required");

            try
            {
                var updated = await _mediaFileService.UpdateAsync(id, file, mediaFileDto);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating media file {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region Delete
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediaFileService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
        #endregion

        #region Download
        [HttpGet("{id}/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Download(int id)
        {
            var media = await _mediaFileService.GetByIdAsync(id);
            if (media == null || string.IsNullOrEmpty(media.Url))
                return NotFound("Media file not found");

            var filePath = Path.Combine(_env.WebRootPath, media.Url);

            if (!System.IO.File.Exists(filePath))
                return NotFound($"File not found: {filePath}");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = "application/octet-stream";
            return File(fileBytes, contentType, Path.GetFileName(filePath));

        }
        #endregion
    }
}
