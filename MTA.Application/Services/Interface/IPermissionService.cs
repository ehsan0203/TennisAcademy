using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IPermissionService
{
    Task<PaginatedResult<PermissionDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default);
    Task<PermissionDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PermissionDto?> GetByTitleAsync(string title, CancellationToken ct = default);
    Task<PermissionDto> CreateAsync(CreatePermissionDto permissionDto, CancellationToken ct = default);
    Task<PermissionDto> UpdateAsync(int id, PermissionDto permissionDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<PermissionStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    Task<IEnumerable<PermissionDto>> GetPermissionsWithRoleCountAsync(CancellationToken ct = default);
    Task<IEnumerable<PermissionDto>> BulkCreateAsync(IEnumerable<CreatePermissionDto> permissionDtos, CancellationToken ct = default);
}

public class PermissionStatisticsDto
{
    public int TotalPermissions { get; set; }
    public int PermissionsWithRoles { get; set; }
    public int PermissionsWithoutRoles { get; set; }
    public double AverageRolesPerPermission { get; set; }
    public int PermissionsThisMonth { get; set; }
    public int PermissionsLastMonth { get; set; }
}
