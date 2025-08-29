using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using Microsoft.Extensions.Logging;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionController> _logger;

    public PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    #region CRUD Operations

    /// <summary>
    /// Get all permissions with pagination and search
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PermissionDto>>> GetPermissions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            var result = await _permissionService.GetAllAsync(page, pageSize, searchTerm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get permission by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PermissionDto>> GetPermission(int id)
    {
        try
        {
            var permission = await _permissionService.GetByIdAsync(id);
            if (permission == null)
                return NotFound("Permission not found");

            return Ok(permission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission with ID: {PermissionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get permission by title
    /// </summary>
    //[HttpGet("by-title/{title}")]
    //public async Task<ActionResult<PermissionDto>> GetPermissionByTitle(string title)
    //{
    //    try
    //    {
    //        var permission = await _permissionService.GetByTitleAsync(title);
    //        if (permission == null)
    //            return NotFound("Permission not found");

    //        return Ok(permission);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error getting permission with title: {Title}", title);
    //        return StatusCode(500, new { message = $"Error getting permission with title: {title}" });

    //    }
    //}

    /// <summary>
    /// Create new permission
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PermissionDto>> CreatePermission([FromBody] PermissionDto permissionDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdPermission = await _permissionService.CreateAsync(permissionDto);
            return CreatedAtAction(nameof(GetPermission), new { id = createdPermission.Id }, createdPermission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permission");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update existing permission
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<PermissionDto>> UpdatePermission(int id, [FromBody] PermissionDto permissionDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedPermission = await _permissionService.UpdateAsync(id, permissionDto);
            if (updatedPermission == null)
                return NotFound("Permission not found");

            return Ok(updatedPermission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permission with ID: {PermissionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete permission
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePermission(int id)
    {
        try
        {
            var result = await _permissionService.DeleteAsync(id);
            if (!result)
                return NotFound("Permission not found");

            return Ok("Permission deleted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting permission with ID: {PermissionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion

    #region Advanced Operations

    ///// <summary>
    ///// Get permission statistics
    ///// </summary>
    //[HttpGet("statistics")]
    //public async Task<ActionResult<PermissionStatisticsDto>> GetPermissionStatistics()
    //{
    //    try
    //    {
    //        var stats = await _permissionService.GetStatisticsAsync();
    //        return Ok(stats);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error getting permission statistics");
    //        return StatusCode(500, "Internal server error");
    //    }
    //}

    ///// <summary>
    ///// Get permissions with role count
    ///// </summary>
    //[HttpGet("with-role-count")]
    //public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissionsWithRoleCount()
    //{
    //    try
    //    {
    //        var permissions = await _permissionService.GetPermissionsWithRoleCountAsync();
    //        return Ok(permissions);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error getting permissions with role count");
    //        return StatusCode(500, "Internal server error");
    //    }
    //}

    /// <summary>
    /// Bulk create permissions
    /// </summary>
    //[HttpPost("bulk")]
    //public async Task<ActionResult<IEnumerable<PermissionDto>>> BulkCreatePermissions([FromBody] IEnumerable<PermissionDto> permissionDtos)
    //{
    //    try
    //    {
    //        if (!ModelState.IsValid)
    //            return BadRequest(ModelState);

    //        var createdPermissions = await _permissionService.BulkCreateAsync(permissionDtos);
    //        return Ok(createdPermissions);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error in bulk create permissions");
    //        return StatusCode(500, "Internal server error");
    //    }
    //}

    #endregion

    #region Search and Filter

    /// <summary>
    /// Search permissions by title or description
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<PaginatedResult<PermissionDto>>> SearchPermissions([FromBody] PermissionSearchDto searchDto)
    {
        try
        {
            var result = await _permissionService.GetAllAsync(
                searchDto.Page, 
                searchDto.PageSize, 
                searchDto.SearchTerm);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching permissions");
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion
}

/// <summary>
/// DTO for permission search
/// </summary>
public class PermissionSearchDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
