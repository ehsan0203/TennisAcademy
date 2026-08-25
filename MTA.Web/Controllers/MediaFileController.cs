using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

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

    [HttpPost("upload")]
    [ProducesResponseType(typeof(CustomJsonResult<MediaFileDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<MediaFileDto>> Upload([FromForm] IFormFile file, [FromForm] MediaFileUploadDto mediaFileDto)
    {
        var created = await _mediaFileService.CreateAsync(file, mediaFileDto);
        return CustomJsonResult<MediaFileDto>.Created(created);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<MediaFileDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<MediaFileDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? typeId = null)
    {
        var result = await _mediaFileService.GetAllAsync(page, pageSize, searchTerm, typeId);
        return CustomJsonResult<PaginatedResult<MediaFileDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<MediaFileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<MediaFileDto>> GetById(int id)
    {
        var file = await _mediaFileService.GetByIdAsync(id);
        return file is null
            ? CustomJsonResult<MediaFileDto>.NotFound($"Media file with ID {id} not found")
            : CustomJsonResult<MediaFileDto>.SuccessResult(file);
    }

    [HttpGet("gifs")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<MediaFileDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<MediaFileDto>>> GetGifs()
    {
        var gifs = await _mediaFileService.GetGifsAsync();
        return CustomJsonResult<IEnumerable<MediaFileDto>>.SuccessResult(gifs);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<MediaFileDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<MediaFileDto>> Update(int id, [FromForm] IFormFile? file, [FromForm] MediaFileUploadDto mediaFileDto)
    {
        var updated = await _mediaFileService.UpdateAsync(id, file, mediaFileDto);
        return CustomJsonResult<MediaFileDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> Delete(int id)
    {
        var deleted = await _mediaFileService.DeleteAsync(id);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Media file with ID {id} not found");
    }
}
