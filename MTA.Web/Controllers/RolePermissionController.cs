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
public class RolePermissionController : ControllerBase
{
    private readonly IRolePermissionService _rolePermissionService;

    public RolePermissionController(IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<RolePermissionDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<RolePermissionDto>>> GetRolePermissions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? roleId = null,
        [FromQuery] int? permissionId = null,
        CancellationToken ct = default)
    {
        var result = await _rolePermissionService.GetAllAsync(page, pageSize, roleId, permissionId, ct);
        return CustomJsonResult<PaginatedResult<RolePermissionDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<RolePermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<RolePermissionDto>> GetRolePermission(int id, CancellationToken ct)
    {
        var rp = await _rolePermissionService.GetByIdAsync(id, ct);
        return rp is null
            ? CustomJsonResult<RolePermissionDto>.NotFound($"Role permission with ID {id} not found")
            : CustomJsonResult<RolePermissionDto>.SuccessResult(rp);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<RolePermissionDto>), StatusCodes.Status201Created)]
    public async Task<CustomJsonResult<RolePermissionDto>> CreateRolePermission([FromBody] CreateRolePermissionDto rolePermissionDto, CancellationToken ct)
    {
        var created = await _rolePermissionService.CreateAsync(rolePermissionDto, ct);
        return CustomJsonResult<RolePermissionDto>.Created(created);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteRolePermission(int id, CancellationToken ct)
    {
        var deleted = await _rolePermissionService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Role permission with ID {id} not found");
    }

    [HttpPost("role/{roleId}/permission/{permissionId}")]
    [ProducesResponseType(typeof(CustomJsonResult<RolePermissionDto>), StatusCodes.Status201Created)]
    public async Task<CustomJsonResult<RolePermissionDto>> AssignPermissionToRole(int roleId, int permissionId, CancellationToken ct)
    {
        var created = await _rolePermissionService.AssignPermissionToRoleAsync(roleId, permissionId, ct);
        return CustomJsonResult<RolePermissionDto>.Created(created);
    }

    [HttpDelete("role/{roleId}/permission/{permissionId}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    public async Task<CustomJsonResult<string>> RemovePermissionFromRole(int roleId, int permissionId, CancellationToken ct)
    {
        await _rolePermissionService.RemovePermissionFromRoleAsync(roleId, permissionId, ct);
        return CustomJsonResult<string>.NoContent();
    }
}
