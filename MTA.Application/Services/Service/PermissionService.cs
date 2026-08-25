using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for Permission operations
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(IUnitOfWork unitOfWork, ILogger<PermissionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all permissions with optional filtering
    /// </summary>
    public async Task<PaginatedResult<PermissionDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Permission>().GetQueryable()
            .AsNoTracking();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                p.Title.Contains(searchTerm) ||
                (p.Description != null && p.Description.Contains(searchTerm)));
        }

        // Get total count
        var totalCount = await query.CountAsync(ct);

        // Apply pagination
        var permissions = await query
            .Include(p => p.PermissionsRoles)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // Map to DTOs
        var permissionDtos = permissions.Select(MapToDto).ToList();

        return new PaginatedResult<PermissionDto>
        {
            Data = permissionDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Get permission by ID
    /// </summary>
    public async Task<PermissionDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var permission = await _unitOfWork.Repository<Permission>().GetQueryable()
            .AsNoTracking()
            .Include(p => p.PermissionsRoles)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return permission != null ? MapToDto(permission) : null;
    }

    /// <summary>
    /// Get permission by title
    /// </summary>
    public async Task<PermissionDto?> GetByTitleAsync(string title, CancellationToken ct = default)
    {
        var permission = await _unitOfWork.Repository<Permission>().GetQueryable()
            .AsNoTracking()
            .Include(p => p.PermissionsRoles)
            .FirstOrDefaultAsync(p => p.Title.ToLower() == title.ToLower(), ct);

        return permission != null ? MapToDto(permission) : null;
    }

    /// <summary>
    /// Create new permission
    /// </summary>
    public async Task<PermissionDto> CreateAsync(CreatePermissionDto permissionDto, CancellationToken ct = default)
    {
        // Check if permission with same title already exists
        var existingPermission = await GetByTitleAsync(permissionDto.Title, ct);
        if (existingPermission != null)
        {
            throw new InvalidOperationException($"Permission with title '{permissionDto.Title}' already exists");
        }

        var permission = new Permission
        {
            Title = permissionDto.Title,
            Description = permissionDto.Description
        };

        await _unitOfWork.Repository<Permission>().AddAsync(permission, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Return the created permission
        return await GetByIdAsync(permission.Id, ct) ?? new PermissionDto
        {
            Id = permission.Id,
            Title = permission.Title,
            Description = permission.Description,
            CreatedAt = permission.CreatedAt,
            UpdatedAt = permission.UpdatedAt
        };
    }

    /// <summary>
    /// Update existing permission
    /// </summary>
    public async Task<PermissionDto> UpdateAsync(int id, PermissionDto permissionDto, CancellationToken ct = default)
    {
        var permission = await _unitOfWork.Repository<Permission>().GetByIdAsync(id, ct);
        if (permission == null)
        {
            throw new InvalidOperationException($"Permission with ID {id} not found");
        }

        // Check if title is being changed and if it conflicts with existing permissions
        if (permission.Title != permissionDto.Title)
        {
            var existingPermission = await GetByTitleAsync(permissionDto.Title, ct);
            if (existingPermission != null && existingPermission.Id != id)
            {
                throw new InvalidOperationException($"Permission with title '{permissionDto.Title}' already exists");
            }
        }

        // Update properties
        permission.Title = permissionDto.Title;
        permission.Description = permissionDto.Description;

        await _unitOfWork.Repository<Permission>().UpdateAsync(permission, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct) ?? permissionDto;
    }

    /// <summary>
    /// Delete permission
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var permission = await _unitOfWork.Repository<Permission>().GetByIdAsync(id, ct);
        if (permission == null)
        {
            return false;
        }

        // Check if permission is assigned to any roles
        var hasRoles = await _unitOfWork.Repository<PermissionsRole>().GetQueryable()
            .AnyAsync(rp => rp.PermissionId == id, ct);

        if (hasRoles)
        {
            throw new InvalidOperationException($"Cannot delete permission '{permission.Title}' as it is assigned to one or more roles");
        }

        await _unitOfWork.Repository<Permission>().DeleteAsync(id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    /// <summary>
    /// Get permission statistics
    /// </summary>
    public async Task<PermissionStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        var permissions = await _unitOfWork.Repository<Permission>().GetQueryable()
            .AsNoTracking()
            .Include(p => p.PermissionsRoles)
            .ToListAsync(ct);

        var totalPermissions = permissions.Count;
        var permissionsWithRoles = permissions.Count(p => p.PermissionsRoles.Any());
        var permissionsWithoutRoles = totalPermissions - permissionsWithRoles;

        var totalRoleAssignments = permissions.Sum(p => p.PermissionsRoles.Count);
        var averageRolesPerPermission = totalPermissions > 0 ? (double)totalRoleAssignments / totalPermissions : 0;

        // Get permissions created this month and last month
        var now = DateTime.UtcNow;
        var thisMonth = new DateTime(now.Year, now.Month, 1);
        var lastMonth = thisMonth.AddMonths(-1);

        var permissionsThisMonth = permissions.Count(p => p.CreatedAt >= thisMonth);
        var permissionsLastMonth = permissions.Count(p => p.CreatedAt >= lastMonth && p.CreatedAt < thisMonth);

        return new PermissionStatisticsDto
        {
            TotalPermissions = totalPermissions,
            PermissionsWithRoles = permissionsWithRoles,
            PermissionsWithoutRoles = permissionsWithoutRoles,
            AverageRolesPerPermission = Math.Round(averageRolesPerPermission, 2),
            PermissionsThisMonth = permissionsThisMonth,
            PermissionsLastMonth = permissionsLastMonth
        };
    }

    /// <summary>
    /// Get permissions with role count
    /// </summary>
    public async Task<IEnumerable<PermissionDto>> GetPermissionsWithRoleCountAsync(CancellationToken ct = default)
    {
        var permissions = await _unitOfWork.Repository<Permission>()
            .GetQueryable()
            .AsNoTracking()
            .Include(p => p.PermissionsRoles)
            .ToListAsync(ct);

        return permissions.Select(MapToDto);
    }

    /// <summary>
    /// Bulk create permissions
    /// </summary>
    public async Task<IEnumerable<PermissionDto>> BulkCreateAsync(IEnumerable<CreatePermissionDto> permissionDtos, CancellationToken ct = default)
    {
        var createdPermissions = new List<PermissionDto>();

        foreach (var permissionDto in permissionDtos)
        {
            try
            {
                var created = await CreateAsync(permissionDto, ct);
                createdPermissions.Add(created);
            }
            catch (Exception ex)
            {
                // Intentional: skip individual failures and continue the batch
                _logger.LogWarning(ex, "Failed to create permission with title: {Title}", permissionDto.Title);
            }
        }

        return createdPermissions;
    }

    #region Helper Methods

    /// <summary>
    /// Map Permission entity to PermissionDto
    /// </summary>
    private PermissionDto MapToDto(Permission permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,
            Title = permission.Title,
            Description = permission.Description,
            RoleCount = permission.PermissionsRoles?.Count ?? 0,
            CreatedAt = permission.CreatedAt,
            UpdatedAt = permission.UpdatedAt
        };
    }

    #endregion
}
