using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for RolePermission operations
/// </summary>
public interface IRolePermissionService
{
    /// <summary>
    /// Get all role permissions with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="roleId">Filter by role ID</param>
    /// <param name="permissionId">Filter by permission ID</param>
    /// <returns>Paginated list of role permissions</returns>
    Task<PaginatedResult<RolePermissionDto>> GetAllAsync(int page = 1, int pageSize = 10, int? roleId = null, int? permissionId = null);
    
    /// <summary>
    /// Get role permission by ID
    /// </summary>
    /// <param name="id">Role permission ID</param>
    /// <returns>Role permission details</returns>
    Task<RolePermissionDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get role permissions by role ID
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>List of role permissions</returns>
    Task<IEnumerable<RolePermissionDto>> GetByRoleAsync(int roleId);
    
    /// <summary>
    /// Get role permissions by permission ID
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>List of role permissions</returns>
    Task<IEnumerable<RolePermissionDto>> GetByPermissionAsync(int permissionId);
    
    /// <summary>
    /// Check if role has specific permission
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>True if role has permission</returns>
    Task<bool> RoleHasPermissionAsync(int roleId, int permissionId);
    
    /// <summary>
    /// Create new role permission
    /// </summary>
    /// <param name="rolePermissionDto">Role permission data</param>
    /// <returns>Created role permission</returns>
    Task<RolePermissionDto> CreateAsync(RolePermissionDto rolePermissionDto);
    
    /// <summary>
    /// Update existing role permission
    /// </summary>
    /// <param name="id">Role permission ID</param>
    /// <param name="rolePermissionDto">Updated role permission data</param>
    /// <returns>Updated role permission</returns>
    Task<RolePermissionDto> UpdateAsync(int id, RolePermissionDto rolePermissionDto);
    
    /// <summary>
    /// Delete role permission
    /// </summary>
    /// <param name="id">Role permission ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Assign permission to role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>Created role permission</returns>
    Task<RolePermissionDto> AssignPermissionToRoleAsync(int roleId, int permissionId);
    
    /// <summary>
    /// Remove permission from role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>True if removed successfully</returns>
    Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);
    
    /// <summary>
    /// Bulk assign permissions to role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionIds">List of permission IDs</param>
    /// <returns>List of created role permissions</returns>
    Task<IEnumerable<RolePermissionDto>> BulkAssignPermissionsToRoleAsync(int roleId, IEnumerable<int> permissionIds);
    
    /// <summary>
    /// Bulk remove permissions from role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionIds">List of permission IDs</param>
    /// <returns>True if all permissions removed successfully</returns>
    Task<bool> BulkRemovePermissionsFromRoleAsync(int roleId, IEnumerable<int> permissionIds);
    
}

/// <summary>
/// Role permission statistics DTO
/// </summary>
public class RolePermissionStatisticsDto
{
    public int TotalRolePermissions { get; set; }
    public int RolesWithPermissions { get; set; }
    public int RolesWithoutPermissions { get; set; }
    public int PermissionsAssignedToRoles { get; set; }
    public int PermissionsNotAssignedToRoles { get; set; }
    public double AveragePermissionsPerRole { get; set; }
    public double AverageRolesPerPermission { get; set; }
    public int RolePermissionsThisMonth { get; set; }
    public int RolePermissionsLastMonth { get; set; }
}
