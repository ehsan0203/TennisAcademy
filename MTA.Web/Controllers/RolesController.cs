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
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<RoleDto>>> GetRoles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        var result = await _roleService.GetAllAsync(page, pageSize, searchTerm, ct);
        return CustomJsonResult<PaginatedResult<RoleDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<RoleDto>> GetRole(int id, CancellationToken ct)
    {
        var role = await _roleService.GetByIdAsync(id, ct);
        return role is null
            ? CustomJsonResult<RoleDto>.NotFound($"Role with ID {id} not found")
            : CustomJsonResult<RoleDto>.SuccessResult(role);
    }
}
