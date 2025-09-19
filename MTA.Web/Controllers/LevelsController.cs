using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing tennis skill levels
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
//[Authorize(Policy = "RolesAdminCoach")]
public class LevelsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LevelsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all available skill levels
    /// </summary>
    /// <returns>List of all skill levels</returns>
    /// <response code="200">Returns the list of skill levels</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<LevelDto>>> GetLevels()
    {
        try
        {
            var levels = await _unitOfWork.Repository<Level>().GetAllAsync();
            var levelDtos = levels.Select(l =>
            {
                var dto = _mapper.Map<LevelDto>(l);
                dto.CourseCount = l.Courses?.Count ?? 0;
                dto.UserCount = l.Profiles?.Count ?? 0;
                return dto;
            });

            return Ok(levelDtos);
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging here
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific skill level by ID
    /// </summary>
    /// <param name="id">The ID of the skill level</param>
    /// <returns>The requested skill level</returns>
    /// <response code="200">Returns the requested skill level</response>
    /// <response code="404">If the skill level was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LevelDto>> GetLevel(int id)
    {
        try
        {
            var level = await _unitOfWork.Repository<Level>().GetByIdAsync(id);
            if (level == null)
                return NotFound($"Level with ID {id} not found");

            var dto = _mapper.Map<LevelDto>(level);
            dto.CourseCount = level.Courses?.Count ?? 0;
            dto.UserCount = level.Profiles?.Count ?? 0;

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new skill level
    /// </summary>
    /// <param name="levelDto">The skill level data</param>
    /// <returns>The created skill level</returns>
    /// <response code="201">Returns the newly created skill level</response>
    /// <response code="400">If the skill level data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LevelDto>> CreateLevel([FromBody] CreateLevelDto levelDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var level = _mapper.Map<Level>(levelDto);
            var createdLevel = await _unitOfWork.Repository<Level>().AddAsync(level);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<LevelDto>(createdLevel);
            resultDto.CourseCount = 0;
            resultDto.UserCount = 0;

            return CreatedAtAction(nameof(GetLevel), new { id = resultDto.Id }, resultDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing skill level
    /// </summary>
    /// <param name="id">The ID of the skill level to update</param>
    /// <param name="levelDto">The updated skill level data</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the skill level was updated successfully</response>
    /// <response code="400">If the skill level data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="404">If the skill level was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateLevel(int id, [FromBody] LevelDto levelDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var existingLevel = await _unitOfWork.Repository<Level>().GetByIdAsync(id);
            if (existingLevel == null)
                return NotFound($"Level with ID {id} not found");

            _mapper.Map(levelDto, existingLevel);
            existingLevel.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Level>().UpdateAsync(existingLevel);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a skill level
    /// </summary>
    /// <param name="id">The ID of the skill level to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">If the skill level was deleted successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have required role</response>
    /// <response code="404">If the skill level was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteLevel(int id)
    {
        try
        {
            var level = await _unitOfWork.Repository<Level>().GetByIdAsync(id);
            if (level == null)
                return NotFound($"Level with ID {id} not found");

            var deleted = await _unitOfWork.Repository<Level>().DeleteAsync(id);
            if (!deleted)
                return StatusCode(500, "Failed to delete level");

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
