using MTA.Application.DTOs;
using MTA.Domain.Entities;

namespace MTA.Application.Services;

public interface IRoleService
{
    Task<PaginatedResult<RoleDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default);
    Task<RoleDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<RoleDto?> GetByTitleAsync(string title, CancellationToken ct = default);
    Task<RoleDto> CreateAsync(RoleDto roleDto, CancellationToken ct = default);
    Task<RoleDto> UpdateAsync(int id, RoleDto roleDto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<Role> GetDefaultStudentRoleAsync(CancellationToken ct = default);
}
