using AutoMapper;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for RolePermission operations
/// </summary>
public class RolePermissionService : IRolePermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RolePermissionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all role permissions with optional filtering
    /// </summary>
    public async Task<PaginatedResult<RolePermissionDto>> GetAllAsync(int page = 1, int pageSize = 10, int? roleId = null, int? permissionId = null)
    {
        var query = _unitOfWork.Repository<PermissionsRole>().GetQueryable();

        // Apply filters
        if (roleId.HasValue)
        {
            query = query.Where(rp => rp.RoleId == roleId.Value);
        }

        if (permissionId.HasValue)
        {
            query = query.Where(rp => rp.PermissionId == permissionId.Value);
        }

        // Get total count
        var totalCount = await _unitOfWork.Repository<PermissionsRole>().CountAsync(query);

        // Apply pagination
        var rolePermissions = await _unitOfWork.Repository<PermissionsRole>().GetPagedAsync(query, page, pageSize);

        // Map to DTOs with additional data
        var rolePermissionDtos = rolePermissions.Select(rp => _mapper.Map<RolePermissionDto>(rp)).ToList();

        return new PaginatedResult<RolePermissionDto>
        {
            Data = rolePermissionDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    /// <summary>
    /// Get role permission by ID
    /// </summary>
    public async Task<RolePermissionDto?> GetByIdAsync(int id)
    {
        var rolePermission = await _unitOfWork.Repository<PermissionsRole>().GetByIdAsync(id);
        return rolePermission != null ? _mapper.Map<RolePermissionDto>(rolePermission) : null;
    }

    /// <summary>
    /// Get role permissions by role ID
    /// </summary>
    public async Task<IEnumerable<RolePermissionDto>> GetByRoleAsync(int roleId)
    {
        var rolePermissions = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync(rp => rp.RoleId == roleId);
        return rolePermissions.Select(rp => _mapper.Map<RolePermissionDto>(rp));
    }

    /// <summary>
    /// Get role permissions by permission ID
    /// </summary>
    public async Task<IEnumerable<RolePermissionDto>> GetByPermissionAsync(int permissionId)
    {
        var rolePermissions = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync(rp => rp.PermissionId == permissionId);
        return rolePermissions.Select(rp => _mapper.Map<RolePermissionDto>(rp));
    }

    /// <summary>
    /// Check if role has specific permission
    /// </summary>
    public async Task<bool> RoleHasPermissionAsync(int roleId, int permissionId)
    {
        var rolePermission = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync(rp => 
            rp.RoleId == roleId && rp.PermissionId == permissionId);
        return rolePermission.Any();
    }

    /// <summary>
    /// Create new role permission
    /// </summary>
    public async Task<RolePermissionDto> CreateAsync(CreateRolePermissionDto rolePermissionDto)
    {
        // Check if role permission already exists
        var existingRolePermission = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync(rp => 
            rp.RoleId == rolePermissionDto.RoleId && rp.PermissionId == rolePermissionDto.PermissionId);
        
        if (existingRolePermission.Any())
        {
            throw new InvalidOperationException($"Role permission already exists for Role ID {rolePermissionDto.RoleId} and Permission ID {rolePermissionDto.PermissionId}");
        }

        var rolePermission = _mapper.Map<PermissionsRole>(rolePermissionDto);
        rolePermission.CreatedAt = DateTime.UtcNow;
        
        var createdRolePermission = await _unitOfWork.Repository<PermissionsRole>().AddAsync(rolePermission);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<RolePermissionDto>(createdRolePermission);
    }

    /// <summary>
    /// Update existing role permission
    /// </summary>
    public async Task<RolePermissionDto> UpdateAsync(int id, RolePermissionDto rolePermissionDto)
    {
        var existingRolePermission = await _unitOfWork.Repository<PermissionsRole>().GetByIdAsync(id);
        if (existingRolePermission == null)
            throw new ArgumentException($"Role permission with ID {id} not found");

        // Check if the new combination already exists (excluding current record)
        var duplicateRolePermission = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync(rp => 
            rp.RoleId == rolePermissionDto.RoleId && 
            rp.PermissionId == rolePermissionDto.PermissionId && 
            rp.Id != id);
        
        if (duplicateRolePermission.Any())
        {
            throw new InvalidOperationException($"Role permission already exists for Role ID {rolePermissionDto.RoleId} and Permission ID {rolePermissionDto.PermissionId}");
        }

        // Update only allowed fields
        existingRolePermission.RoleId = rolePermissionDto.RoleId;
        existingRolePermission.PermissionId = rolePermissionDto.PermissionId;
        existingRolePermission.UpdatedAt = DateTime.UtcNow;

        var updatedRolePermission = await _unitOfWork.Repository<PermissionsRole>().UpdateAsync(existingRolePermission);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<RolePermissionDto>(updatedRolePermission);
    }

    /// <summary>
    /// Delete role permission
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var rolePermission = await _unitOfWork.Repository<PermissionsRole>().GetByIdAsync(id);
        if (rolePermission == null)
            return false;

        await _unitOfWork.Repository<PermissionsRole>().DeleteAsync(rolePermission.Id);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Assign permission to role
    /// </summary>
    public async Task<RolePermissionDto> AssignPermissionToRoleAsync(int roleId, int permissionId)
    {
        // Check if role permission already exists
        var existingRolePermission = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync(rp => 
            rp.RoleId == roleId && rp.PermissionId == permissionId);
        
        if (existingRolePermission.Any())
        {
            throw new InvalidOperationException($"Permission {permissionId} is already assigned to role {roleId}");
        }

        var rolePermission = new PermissionsRole
        {
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedAt = DateTime.UtcNow
        };
        
        var createdRolePermission = await _unitOfWork.Repository<PermissionsRole>().AddAsync(rolePermission);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<RolePermissionDto>(createdRolePermission);
    }

    /// <summary>
    /// Remove permission from role
    /// </summary>
    public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
    {
        var rolePermission = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync(rp => 
            rp.RoleId == roleId && rp.PermissionId == permissionId);
        
        if (!rolePermission.Any())
        {
            return false; // Permission not assigned to role
        }

        foreach (var rp in rolePermission)
        {
            await _unitOfWork.Repository<PermissionsRole>().DeleteAsync(rp.Id);
        }
        
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Bulk assign permissions to role
    /// </summary>
    public async Task<IEnumerable<RolePermissionDto>> BulkAssignPermissionsToRoleAsync(int roleId, IEnumerable<int> permissionIds)
    {
        var createdRolePermissions = new List<RolePermissionDto>();
        var existingPermissions = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync(rp => rp.RoleId == roleId);
        var existingPermissionIds = existingPermissions.Select(rp => rp.PermissionId).ToHashSet();

        foreach (var permissionId in permissionIds)
        {
            if (!existingPermissionIds.Contains(permissionId))
            {
                var rolePermission = new PermissionsRole
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    CreatedAt = DateTime.UtcNow
                };
                
                var createdRolePermission = await _unitOfWork.Repository<PermissionsRole>().AddAsync(rolePermission);
                createdRolePermissions.Add(_mapper.Map<RolePermissionDto>(createdRolePermission));
            }
        }
        
        await _unitOfWork.SaveChangesAsync();
        return createdRolePermissions;
    }

    /// <summary>
    /// Bulk remove permissions from role
    /// </summary>
    public async Task<bool> BulkRemovePermissionsFromRoleAsync(int roleId, IEnumerable<int> permissionIds)
    {
        var rolePermissions = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync(rp => 
            rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId));
        
        if (!rolePermissions.Any())
        {
            return false; // No permissions to remove
        }

        foreach (var rp in rolePermissions)
        {
            await _unitOfWork.Repository<PermissionsRole>().DeleteAsync(rp.Id);
        }
        
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get role permission statistics
    /// </summary>
    public async Task<RolePermissionStatisticsDto> GetStatisticsAsync()
    {
        var allRolePermissions = await _unitOfWork.Repository<PermissionsRole>().GetAllAsync();
        var allRoles = await _unitOfWork.Repository<Role>().GetAllAsync();
        var allPermissions = await _unitOfWork.Repository<Permission>().GetAllAsync();

        var rolesWithPermissions = allRoles.Count(r => allRolePermissions.Any(rp => rp.RoleId == r.Id));
        var rolesWithoutPermissions = allRoles.Count() - rolesWithPermissions;

        var permissionsAssignedToRoles = allPermissions.Count(p => allRolePermissions.Any(rp => rp.PermissionId == p.Id));
        var permissionsNotAssignedToRoles = allPermissions.Count() - permissionsAssignedToRoles;

        var averagePermissionsPerRole = allRoles.Any() ? (double)allRolePermissions.Count() / allRoles.Count() : 0;
        var averageRolesPerPermission = allPermissions.Any() ? (double)allRolePermissions.Count() / allPermissions.Count() : 0;

        var currentDate = DateTime.UtcNow;
        var rolePermissionsThisMonth = allRolePermissions.Count(rp => 
            rp.CreatedAt.Month == currentDate.Month && rp.CreatedAt.Year == currentDate.Year);
        var rolePermissionsLastMonth = allRolePermissions.Count(rp => 
            rp.CreatedAt.Month == currentDate.AddMonths(-1).Month && rp.CreatedAt.Year == currentDate.AddMonths(-1).Year);
        
        return new RolePermissionStatisticsDto
        {
            TotalRolePermissions = allRolePermissions.Count(),
            RolesWithPermissions = rolesWithPermissions,
            RolesWithoutPermissions = rolesWithoutPermissions,
            PermissionsAssignedToRoles = permissionsAssignedToRoles,
            PermissionsNotAssignedToRoles = permissionsNotAssignedToRoles,
            AveragePermissionsPerRole = averagePermissionsPerRole,
            AverageRolesPerPermission = averageRolesPerPermission,
            RolePermissionsThisMonth = rolePermissionsThisMonth,
            RolePermissionsLastMonth = rolePermissionsLastMonth
        };
    }
}
