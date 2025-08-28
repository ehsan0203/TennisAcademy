using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Permission operations
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Get all permissions with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title or description</param>
    /// <returns>Paginated list of permissions</returns>
    Task<PaginatedResult<PermissionDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null);
    
    /// <summary>
    /// Get permission by ID
    /// </summary>
    /// <param name="id">Permission ID</param>
    /// <returns>Permission details</returns>
    Task<PermissionDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get permission by title
    /// </summary>
    /// <param name="title">Permission title</param>
    /// <returns>Permission details</returns>
    Task<PermissionDto?> GetByTitleAsync(string title);
    
    /// <summary>
    /// Create new permission
    /// </summary>
    /// <param name="permissionDto">Permission data</param>
    /// <returns>Created permission</returns>
    Task<PermissionDto> CreateAsync(PermissionDto permissionDto);
    
    /// <summary>
    /// Update existing permission
    /// </summary>
    /// <param name="id">Permission ID</param>
    /// <param name="permissionDto">Updated permission data</param>
    /// <returns>Updated permission</returns>
    Task<PermissionDto> UpdateAsync(int id, PermissionDto permissionDto);
    
    /// <summary>
    /// Delete permission
    /// </summary>
    /// <param name="id">Permission ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Get permission statistics
    /// </summary>
    /// <returns>Permission statistics</returns>
    Task<PermissionStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get permissions with role count
    /// </summary>
    /// <returns>List of permissions with role counts</returns>
    Task<IEnumerable<PermissionDto>> GetPermissionsWithRoleCountAsync();
    
    /// <summary>
    /// Bulk create permissions
    /// </summary>
    /// <param name="permissionDtos">List of permission data</param>
    /// <returns>List of created permissions</returns>
    Task<IEnumerable<PermissionDto>> BulkCreateAsync(IEnumerable<PermissionDto> permissionDtos);
}

/// <summary>
/// Permission statistics DTO
/// </summary>
public class PermissionStatisticsDto
{
    public int TotalPermissions { get; set; }
    public int PermissionsWithRoles { get; set; }
    public int PermissionsWithoutRoles { get; set; }
    public double AverageRolesPerPermission { get; set; }
    public int PermissionsThisMonth { get; set; }
    public int PermissionsLastMonth { get; set; }
}
