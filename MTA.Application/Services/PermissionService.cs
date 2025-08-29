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
    public async Task<PaginatedResult<PermissionDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        try
        {
            var query = _unitOfWork.Repository<Permission>().GetQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => 
                    p.Title.Contains(searchTerm) || 
                    (p.Description != null && p.Description.Contains(searchTerm)));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var permissions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.PermissionsRoles)
                .ToListAsync();

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions with page {Page}, pageSize {PageSize}, searchTerm {SearchTerm}", 
                page, pageSize, searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Get permission by ID
    /// </summary>
    public async Task<PermissionDto?> GetByIdAsync(int id)
    {
        try
        {
            var permission = await _unitOfWork.Repository<Permission>().GetQueryable()
                .Include(p => p.PermissionsRoles)
                .FirstOrDefaultAsync(p => p.Id == id);

            return permission != null ? MapToDto(permission) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission with ID: {PermissionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get permission by title
    /// </summary>
    public async Task<PermissionDto?> GetByTitleAsync(string title)
    {
        try
        {
            var permission = await _unitOfWork.Repository<Permission>().GetQueryable()
                .Include(p => p.PermissionsRoles)
                .FirstOrDefaultAsync(p => p.Title.ToLower() == title.ToLower());

            return permission != null ? MapToDto(permission) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission with title: {Title}", title);
            throw;
        }
        }

    /// <summary>
    /// Create new permission
    /// </summary>
    public async Task<PermissionDto> CreateAsync(PermissionDto permissionDto)
    {
        try
        {
            // Check if permission with same title already exists
            var existingPermission = await GetByTitleAsync(permissionDto.Title);
            if (existingPermission != null)
            {
                throw new InvalidOperationException($"Permission with title '{permissionDto.Title}' already exists");
            }

            var permission = new Permission
            {
                Title = permissionDto.Title,
                Description = permissionDto.Description
            };

            await _unitOfWork.Repository<Permission>().AddAsync(permission);
            await _unitOfWork.SaveChangesAsync();

            // Return the created permission
            return await GetByIdAsync(permission.Id) ?? permissionDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permission with title: {Title}", permissionDto.Title);
            throw;
        }
    }

    /// <summary>
    /// Update existing permission
    /// </summary>
    public async Task<PermissionDto> UpdateAsync(int id, PermissionDto permissionDto)
    {
        try
        {
            var permission = await _unitOfWork.Repository<Permission>().GetByIdAsync(id);
            if (permission == null)
            {
                throw new InvalidOperationException($"Permission with ID {id} not found");
            }

            // Check if title is being changed and if it conflicts with existing permissions
            if (permission.Title != permissionDto.Title)
            {
                var existingPermission = await GetByTitleAsync(permissionDto.Title);
                if (existingPermission != null && existingPermission.Id != id)
                {
                    throw new InvalidOperationException($"Permission with title '{permissionDto.Title}' already exists");
                }
            }

            // Update properties
            permission.Title = permissionDto.Title;
            permission.Description = permissionDto.Description;
            permission.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Permission>().UpdateAsync(permission);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id) ?? permissionDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permission with ID: {PermissionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete permission
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var permission = await _unitOfWork.Repository<Permission>().GetByIdAsync(id);
            if (permission == null)
            {
                return false;
            }

            // Check if permission is assigned to any roles
            var hasRoles = await _unitOfWork.Repository<PermissionsRole>().GetQueryable()
                .AnyAsync(rp => rp.PermissionId == id);

            if (hasRoles)
            {
                throw new InvalidOperationException($"Cannot delete permission '{permission.Title}' as it is assigned to one or more roles");
            }

            await _unitOfWork.Repository<Permission>().DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting permission with ID: {PermissionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get permission statistics
    /// </summary>
    public async Task<PermissionStatisticsDto> GetStatisticsAsync()
    {
        try
        {
            var permissions = await _unitOfWork.Repository<Permission>().GetQueryable()
                .Include(p => p.PermissionsRoles)
                .ToListAsync();

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission statistics");
            throw;
        }
    }

    /// <summary>
    /// Get permissions with role count
    /// </summary>
    public async Task<IEnumerable<PermissionDto>> GetPermissionsWithRoleCountAsync()
    {
        try
        {
            var permissions = await _unitOfWork.Repository<Permission>()
                .GetQueryable()
                .Include(p => p.PermissionsRoles)  
                .ToListAsync();

            return permissions.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions with role count");
            throw;
        }
    }

    /// <summary>
    /// Bulk create permissions
    /// </summary>
    public async Task<IEnumerable<PermissionDto>> BulkCreateAsync(IEnumerable<PermissionDto> permissionDtos)
    {
        try
        {
            var createdPermissions = new List<PermissionDto>();

            foreach (var permissionDto in permissionDtos)
            {
                try
                {
                    var created = await CreateAsync(permissionDto);
                    createdPermissions.Add(created);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to create permission with title: {Title}", permissionDto.Title);
                    // Continue with other permissions
                }
            }

            return createdPermissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk create permissions");
            throw;
        }
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
