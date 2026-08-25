using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<PermissionDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<PermissionDto>>> GetPermissions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        var result = await _permissionService.GetAllAsync(page, pageSize, searchTerm, ct);
        return CustomJsonResult<PaginatedResult<PermissionDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<PermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<PermissionDto>> GetPermission(int id, CancellationToken ct)
    {
        var permission = await _permissionService.GetByIdAsync(id, ct);
        return permission is null
            ? CustomJsonResult<PermissionDto>.NotFound($"Permission with ID {id} not found")
            : CustomJsonResult<PermissionDto>.SuccessResult(permission);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<PermissionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<PermissionDto>> CreatePermission([FromBody] CreatePermissionDto permissionDto, CancellationToken ct)
    {
        var created = await _permissionService.CreateAsync(permissionDto, ct);
        return CustomJsonResult<PermissionDto>.Created(created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<PermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<PermissionDto>> UpdatePermission(int id, [FromBody] PermissionDto permissionDto, CancellationToken ct)
    {
        var updated = await _permissionService.UpdateAsync(id, permissionDto, ct);
        return CustomJsonResult<PermissionDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeletePermission(int id, CancellationToken ct)
    {
        var deleted = await _permissionService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Permission with ID {id} not found");
    }
}
