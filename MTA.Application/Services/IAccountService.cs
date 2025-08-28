using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Account operations
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Get all accounts with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for email or name</param>
    /// <param name="roleId">Filter by role ID</param>
    /// <param name="statusId">Filter by status ID</param>
    /// <returns>Paginated list of accounts</returns>
    Task<PaginatedResult<AccountDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? roleId = null, int? statusId = null);
    
    /// <summary>
    /// Get account by ID
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <returns>Account details</returns>
    Task<AccountDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get account by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>Account details</returns>
    Task<AccountDto?> GetByEmailAsync(string email);
    
    /// <summary>
    /// Create new account
    /// </summary>
    /// <param name="accountDto">Account data</param>
    /// <returns>Created account</returns>
    Task<AccountDto> CreateAsync(AccountDto accountDto);
    
    /// <summary>
    /// Update existing account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="accountDto">Updated account data</param>
    /// <returns>Updated account</returns>
    Task<AccountDto> UpdateAsync(int id, AccountDto accountDto);
    
    /// <summary>
    /// Delete account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Activate/deactivate account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="isActive">Active status</param>
    /// <returns>Updated account</returns>
    Task<AccountDto> SetActiveStatusAsync(int id, bool isActive);
    
    /// <summary>
    /// Change account role
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="roleId">New role ID</param>
    /// <returns>Updated account</returns>
    Task<AccountDto> ChangeRoleAsync(int id, int roleId);
    
    /// <summary>
    /// Change account status
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="statusId">New status ID</param>
    /// <returns>Updated account</returns>
    Task<AccountDto> ChangeStatusAsync(int id, int statusId);
}

/// <summary>
/// Paginated result wrapper
/// </summary>
/// <typeparam name="T">Type of items</typeparam>
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
