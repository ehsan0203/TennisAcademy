using Microsoft.AspNetCore.Http;
using MTA.Application.DTOs;
using MTA.Application.DTOs.User;

namespace MTA.Application.Services;

public interface IAccountService
{
    Task<PaginatedResult<AccountDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? roleId = null, bool? isActive = null, CancellationToken ct = default);
    Task<AccountDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AccountDto?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<CurrentUserDto?> GetCurrentUserAsync(int accountId, CancellationToken ct = default);
    Task<AccountDto> CreateAsync(CreateAccountDto accountDto, CancellationToken ct = default);
    Task<AccountDto?> UpdateAsync(int id, UpdateAccountDto updateDto, CancellationToken ct = default);
    Task<CurrentUserDto?> UpdateCurrentUserAsync(int accountId, UpdateCurrentUserDto updateDto, CancellationToken ct = default);
    Task<string?> UploadProfileImageAsync(int accountId, IFormFile profileImage, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword, CancellationToken ct = default);
    Task<AccountDto?> SetActiveStatusAsync(int id, bool isActive, CancellationToken ct = default);
    Task<AccountDto?> ChangeRoleAsync(int id, int roleId, CancellationToken ct = default);
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
