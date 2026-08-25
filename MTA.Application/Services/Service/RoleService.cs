using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services.Service;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<RoleDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Role>().GetQueryable().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(r => r.Title.Contains(searchTerm));

        var totalCount = await query.CountAsync(ct);
        var roles = await query
            .OrderBy(r => r.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.Accounts)
            .Include(r => r.RolePermissions)
            .AsSplitQuery()
            .ToListAsync(ct);

        return new PaginatedResult<RoleDto>
        {
            Data = roles.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetQueryable()
            .AsNoTracking()
            .Include(r => r.Accounts)
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        return role is null ? null : MapToDto(role);
    }

    public async Task<RoleDto?> GetByTitleAsync(string title, CancellationToken ct = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Title.ToLower() == title.ToLower(), ct);

        return role is null ? null : MapToDto(role);
    }

    public async Task<RoleDto> CreateAsync(RoleDto roleDto, CancellationToken ct = default)
    {
        var role = new Role { Title = roleDto.Title };
        await _unitOfWork.Repository<Role>().AddAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(role);
    }

    public async Task<RoleDto> UpdateAsync(int id, RoleDto roleDto, CancellationToken ct = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Role with ID {id} not found");

        role.Title = roleDto.Title;
        await _unitOfWork.Repository<Role>().UpdateAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(role);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var deleted = await _unitOfWork.Repository<Role>().DeleteAsync(id, ct);
        if (deleted) await _unitOfWork.SaveChangesAsync(ct);
        return deleted;
    }

    // Used by AuthService during registration — finds Student or User role
    public async Task<Role> GetDefaultStudentRoleAsync(CancellationToken ct = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Title.ToLower() == "student", ct)
            ?? await _unitOfWork.Repository<Role>().GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Title.ToLower() == "user" || r.Title.ToLower() == "کاربر", ct);

        return role ?? throw new InvalidOperationException("No suitable default role found (student or user).");
    }

    private static RoleDto MapToDto(Role role) => new()
    {
        Id = role.Id,
        Title = role.Title,
        AccountCount = role.Accounts?.Count ?? 0,
        PermissionCount = role.RolePermissions?.Count ?? 0
    };
}
