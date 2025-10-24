using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Role operations
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Get all roles with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title</param>
    /// <returns>Paginated list of roles</returns>
    Task<PaginatedResult<RoleDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null);
    
    /// <summary>
    /// Get role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Role details</returns>
    Task<RoleDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get role by title
    /// </summary>
    /// <param name="title">Role title</param>
    /// <returns>Role details</returns>
    Task<RoleDto?> GetByTitleAsync(string title);
    
    /// <summary>
    /// Create new role
    /// </summary>
    /// <param name="roleDto">Role data</param>
    /// <returns>Created role</returns>
    Task<RoleDto> CreateAsync(RoleDto roleDto);
    
    /// <summary>
    /// Update existing role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="roleDto">Updated role data</param>
    /// <returns>Updated role</returns>
    Task<RoleDto> UpdateAsync(int id, RoleDto roleDto);
    
    /// <summary>
    /// Delete role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Get role statistics
    /// </summary>
    /// <returns>Role statistics</returns>
    Task<RoleStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get roles with permission count
    /// </summary>
    /// <returns>List of roles with permission counts</returns>
    Task<IEnumerable<RoleDto>> GetRolesWithPermissionCountAsync();
}

/// <summary>
/// Role statistics DTO
/// </summary>
public class RoleStatisticsDto
{
    public int TotalRoles { get; set; }
    public int RolesWithPermissions { get; set; }
    public int RolesWithoutPermissions { get; set; }
    public double AveragePermissionsPerRole { get; set; }
    public int RolesThisMonth { get; set; }
    public int RolesLastMonth { get; set; }
}
