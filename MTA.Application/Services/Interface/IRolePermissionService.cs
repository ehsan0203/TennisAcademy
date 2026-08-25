using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IRolePermissionService
{
    Task<PaginatedResult<RolePermissionDto>> GetAllAsync(int page = 1, int pageSize = 10, int? roleId = null, int? permissionId = null, CancellationToken ct = default);
    Task<RolePermissionDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<RolePermissionDto>> GetByRoleAsync(int roleId, CancellationToken ct = default);
    Task<IEnumerable<RolePermissionDto>> GetByPermissionAsync(int permissionId, CancellationToken ct = default);
    Task<bool> RoleHasPermissionAsync(int roleId, int permissionId, CancellationToken ct = default);
    Task<RolePermissionDto> CreateAsync(CreateRolePermissionDto rolePermissionDto, CancellationToken ct = default);
    Task<RolePermissionDto> UpdateAsync(int id, RolePermissionDto rolePermissionDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<RolePermissionDto> AssignPermissionToRoleAsync(int roleId, int permissionId, CancellationToken ct = default);
    Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId, CancellationToken ct = default);
    Task<IEnumerable<RolePermissionDto>> BulkAssignPermissionsToRoleAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken ct = default);
    Task<bool> BulkRemovePermissionsFromRoleAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken ct = default);
}

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
