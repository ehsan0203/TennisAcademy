using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing role permissions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
//[Authorize(Roles = "Admin")]
public class RolePermissionController : ControllerBase
{
    private readonly IRolePermissionService _rolePermissionService;

    public RolePermissionController(IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    #region RolePermission CRUD Operations

    /// <summary>
    /// Gets all role permissions with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="roleId">Filter by role ID</param>
    /// <param name="permissionId">Filter by permission ID</param>
    /// <returns>Paginated list of role permissions</returns>
    /// <response code="200">Returns the list of role permissions</response>
    /// <response code="400">If the filter parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<RolePermissionDto>>> GetRolePermissions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? roleId = null,
        [FromQuery] int? permissionId = null)
    {
        try
        {
            var result = await _rolePermissionService.GetAllAsync(page, pageSize, roleId, permissionId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific role permission by ID
    /// </summary>
    /// <param name="id">The ID of the role permission</param>
    /// <returns>The requested role permission</returns>
    /// <response code="200">Returns the requested role permission</response>
    /// <response code="404">If the role permission was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RolePermissionDto>> GetRolePermission(int id)
    {
        try
        {
            var rolePermission = await _rolePermissionService.GetByIdAsync(id);
            if (rolePermission == null)
                return NotFound($"Role permission with ID {id} not found");

            return Ok(rolePermission);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new role permission
    /// </summary>
    /// <param name="rolePermissionDto">The role permission data</param>
    /// <returns>The created role permission</returns>
    /// <response code="201">Returns the newly created role permission</response>
    /// <response code="400">If the role permission data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RolePermissionDto>> CreateRolePermission([FromBody] CreateRolePermissionDto rolePermissionDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRolePermission = await _rolePermissionService.CreateAsync(rolePermissionDto);
            return CreatedAtAction(nameof(GetRolePermission), new { id = createdRolePermission.Id }, createdRolePermission);
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
    /// Updates an existing role permission
    /// </summary>
    /// <param name="id">The ID of the role permission to update</param>
    /// <param name="rolePermissionDto">The updated role permission data</param>
    /// <returns>The updated role permission</returns>
    /// <response code="200">Returns the updated role permission</response>
    /// <response code="400">If the role permission data is invalid</response>
    /// <response code="404">If the role permission was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RolePermissionDto>> UpdateRolePermission(int id, [FromBody] RolePermissionDto rolePermissionDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedRolePermission = await _rolePermissionService.UpdateAsync(id, rolePermissionDto);
            return Ok(updatedRolePermission);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
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
    /// Deletes a role permission
    /// </summary>
    /// <param name="id">The ID of the role permission to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the role permission was deleted successfully</response>
    /// <response code="404">If the role permission was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRolePermission(int id)
    {
        try
        {
            var deleted = await _rolePermissionService.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Role permission with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region RolePermission Queries

    /// <summary>
    /// Gets role permissions by role ID
    /// </summary>
    /// <param name="roleId">The ID of the role</param>
    /// <returns>List of role permissions for the specified role</returns>
    /// <response code="200">Returns the list of role permissions</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("role/{roleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RolePermissionDto>>> GetRolePermissionsByRole(int roleId)
    {
        try
        {
            var rolePermissions = await _rolePermissionService.GetByRoleAsync(roleId);
            return Ok(rolePermissions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets role permissions by permission ID
    /// </summary>
    /// <param name="permissionId">The ID of the permission</param>
    /// <returns>List of role permissions for the specified permission</returns>
    /// <response code="200">Returns the list of role permissions</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("permission/{permissionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RolePermissionDto>>> GetRolePermissionsByPermission(int permissionId)
    {
        try
        {
            var rolePermissions = await _rolePermissionService.GetByPermissionAsync(permissionId);
            return Ok(rolePermissions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if a role has a specific permission
    /// </summary>
    /// <param name="roleId">The ID of the role</param>
    /// <param name="permissionId">The ID of the permission</param>
    /// <returns>True if role has permission</returns>
    /// <response code="200">Returns the result</response>
    /// <response code="500">If there was an internal server error</response>
    //[HttpGet("check")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<bool>> CheckRoleHasPermission(
    //    [FromQuery] int roleId,
    //    [FromQuery] int permissionId)
    //{
    //    try
    //    {
    //        var hasPermission = await _rolePermissionService.RoleHasPermissionAsync(roleId, permissionId);
    //        return Ok(hasPermission);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    #endregion

    #region RolePermission Management Operations

    /// <summary>
    /// Assigns a permission to a role
    /// </summary>
    /// <param name="roleId">The ID of the role</param>
    /// <param name="permissionId">The ID of the permission</param>
    /// <returns>The created role permission</returns>
    /// <response code="201">Returns the newly created role permission</response>
    /// <response code="400">If the permission is already assigned to the role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("assign")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RolePermissionDto>> AssignPermissionToRole(
        [FromQuery] int roleId,
        [FromQuery] int permissionId)
    {
        try
        {
            var createdRolePermission = await _rolePermissionService.AssignPermissionToRoleAsync(roleId, permissionId);
            return CreatedAtAction(nameof(GetRolePermission), new { id = createdRolePermission.Id }, createdRolePermission);
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
    /// Removes a permission from a role
    /// </summary>
    /// <param name="roleId">The ID of the role</param>
    /// <param name="permissionId">The ID of the permission</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the permission was removed successfully</response>
    /// <response code="404">If the permission was not assigned to the role</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("remove")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemovePermissionFromRole(
        [FromQuery] int roleId,
        [FromQuery] int permissionId)
    {
        try
        {
            var removed = await _rolePermissionService.RemovePermissionFromRoleAsync(roleId, permissionId);
            if (!removed)
                return NotFound($"Permission {permissionId} is not assigned to role {roleId}");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    ///// <summary>
    ///// Bulk assigns multiple permissions to a role
    ///// </summary>
    ///// <param name="roleId">The ID of the role</param>
    ///// <param name="permissionIds">List of permission IDs to assign</param>
    ///// <returns>List of created role permissions</returns>
    ///// <response code="201">Returns the newly created role permissions</response>
    ///// <response code="400">If the request data is invalid</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpPost("bulk-assign")]
    //[ProducesResponseType(StatusCodes.Status201Created)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<IEnumerable<RolePermissionDto>>> BulkAssignPermissionsToRole(
    //    [FromQuery] int roleId,
    //    [FromBody] IEnumerable<int> permissionIds)
    //{
    //    try
    //    {
    //        if (permissionIds == null || !permissionIds.Any())
    //            return BadRequest("Permission IDs list cannot be empty");

    //        var createdRolePermissions = await _rolePermissionService.BulkAssignPermissionsToRoleAsync(roleId, permissionIds);
    //        return CreatedAtAction(nameof(GetRolePermissionsByRole), new { roleId }, createdRolePermissions);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    ///// <summary>
    ///// Bulk removes multiple permissions from a role
    ///// </summary>
    ///// <param name="roleId">The ID of the role</param>
    ///// <param name="permissionIds">List of permission IDs to remove</param>
    ///// <returns>No content if successful</returns>
    ///// <response code="204">If the permissions were removed successfully</response>
    ///// <response code="400">If the request data is invalid</response>
    ///// <response code="500">If there was an internal server error</response>
    //[HttpDelete("bulk-remove")]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<IActionResult> BulkRemovePermissionsFromRole(
    //    [FromQuery] int roleId,
    //    [FromBody] IEnumerable<int> permissionIds)
    //{
    //    try
    //    {
    //        if (permissionIds == null || !permissionIds.Any())
    //            return BadRequest("Permission IDs list cannot be empty");

    //        var removed = await _rolePermissionService.BulkRemovePermissionsFromRoleAsync(roleId, permissionIds);
    //        if (!removed)
    //            return NotFound($"No permissions were found to remove for role {roleId}");

    //        return NoContent();
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    #endregion

}
