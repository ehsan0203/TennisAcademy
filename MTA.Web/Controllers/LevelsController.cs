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
public class LevelsController : ControllerBase
{
    private readonly ILevelService _levelService;

    public LevelsController(ILevelService levelService)
    {
        _levelService = levelService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<LevelDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<LevelDto>>> GetLevels(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        var result = await _levelService.GetAllAsync(page, pageSize, searchTerm, ct);
        return CustomJsonResult<PaginatedResult<LevelDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<LevelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<LevelDto>> GetLevel(int id, CancellationToken ct)
    {
        var level = await _levelService.GetByIdAsync(id, ct);
        return level is null
            ? CustomJsonResult<LevelDto>.NotFound($"Level with ID {id} not found")
            : CustomJsonResult<LevelDto>.SuccessResult(level);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<LevelDto>), StatusCodes.Status201Created)]
    public async Task<CustomJsonResult<LevelDto>> CreateLevel([FromBody] LevelDto levelDto, CancellationToken ct)
    {
        var created = await _levelService.CreateAsync(levelDto, ct);
        return CustomJsonResult<LevelDto>.Created(created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<LevelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<LevelDto>> UpdateLevel(int id, [FromBody] LevelDto levelDto, CancellationToken ct)
    {
        var updated = await _levelService.UpdateAsync(id, levelDto, ct);
        return CustomJsonResult<LevelDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteLevel(int id, CancellationToken ct)
    {
        var deleted = await _levelService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Level with ID {id} not found");
    }
}
