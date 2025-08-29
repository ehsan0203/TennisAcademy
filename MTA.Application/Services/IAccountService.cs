using MTA.Application.DTOs;
using MTA.Application.DTOs.User;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Account operations (light & optimized)
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Get all accounts with optional filtering and pagination
    /// </summary>
    Task<PaginatedResult<AccountDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? roleId = null, bool? isActive = null);

    /// <summary>
    /// Get account by ID
    /// </summary>
    Task<AccountDto?> GetByIdAsync(int id);

    /// <summary>
    /// Get account by email
    /// </summary>
    Task<AccountDto?> GetByEmailAsync(string email);

    /// <summary>
    /// Create new account
    /// </summary>
    Task<AccountDto> CreateAsync(CreateAccountDto accountDto);

    /// <summary>
    /// Update account info (name, email, role, status, etc.)
    /// </summary>
    Task<AccountDto?> UpdateAsync(int id, UpdateAccountDto updateDto);

    /// <summary>
    /// Soft delete account
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Change account password
    /// </summary>
    Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword);

    /// <summary>
    /// Activate or deactivate account
    /// </summary>
    Task<AccountDto?> SetActiveStatusAsync(int id, bool isActive);

    /// <summary>
    /// Change account role
    /// </summary>
    Task<AccountDto?> ChangeRoleAsync(int id, int roleId);
}

/// <summary>
/// Paginated result wrapper
/// </summary>
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
