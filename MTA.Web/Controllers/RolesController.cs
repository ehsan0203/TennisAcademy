using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing user roles
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
//[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RolesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all available roles
    /// </summary>
    /// <returns>List of all roles</returns>
    /// <response code="200">Returns the list of roles</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have Admin role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        try
        {
            var roles = await _unitOfWork.Repository<Role>().GetAllAsync();
            var roleDtos = roles.Select(r =>
            {
                var dto = _mapper.Map<RoleDto>(r);
                dto.AccountCount = r.Accounts?.Count ?? 0;
                dto.PermissionCount = r.RolePermissions?.Count ?? 0;
                return dto;
            }).ToList();

            return Ok(roleDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific role by ID
    /// </summary>
    /// <param name="id">The ID of the role</param>
    /// <returns>The requested role</returns>
    /// <response code="200">Returns the requested role</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have Admin role</response>
    /// <response code="404">If the role was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoleDto>> GetRole(int id)
    {
        try
        {
            var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id);
            if (role == null)
                return NotFound($"Role with ID {id} not found");

            var dto = _mapper.Map<RoleDto>(role);
            dto.AccountCount = role.Accounts?.Count ?? 0;
            dto.PermissionCount = role.RolePermissions?.Count ?? 0;

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <param name="roleDto">The role data</param>
    /// <returns>The created role</returns>
    /// <response code="201">Returns the newly created role</response>
    /// <response code="400">If the role data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have Admin role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] RoleDto roleDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var exists = await _unitOfWork.Repository<Role>().AnyAsync(r => r.Title == roleDto.Title);
            if (exists)
                return BadRequest($"Role with title '{roleDto.Title}' already exists.");

            var role = _mapper.Map<Role>(roleDto);

            var createdRole = await _unitOfWork.Repository<Role>().AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<RoleDto>(createdRole);
            resultDto.AccountCount = 0;
            resultDto.PermissionCount = 0;

            return CreatedAtAction(nameof(GetRole), new { id = resultDto.Id }, resultDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] RoleDto roleDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id);
        if (role == null)
            return NotFound($"Role with ID {id} not found");

        var exists = await _unitOfWork.Repository<Role>().AnyAsync(r => r.Title == roleDto.Title && r.Id != id);
        if (exists)
            return BadRequest($"Role with title '{roleDto.Title}' already exists.");

        role.Title = roleDto.Title;

        _unitOfWork.Repository<Role>().UpdateAsync(role);
        await _unitOfWork.SaveChangesAsync();

        var resultDto = _mapper.Map<RoleDto>(role);
        resultDto.AccountCount = role.Accounts?.Count ?? 0;
        resultDto.PermissionCount = role.RolePermissions?.Count ?? 0;

        return Ok(resultDto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id);
        if (role == null)
            return NotFound($"Role with ID {id} not found");

        await _unitOfWork.Repository<Role>().DeleteAsync(role.Id);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
