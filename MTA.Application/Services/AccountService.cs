using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs.User;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for Account operations (light & optimized)
/// </summary>
public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PaginatedResult<AccountDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? roleId = null, bool? isActive = null)
    {
        try
        {
            var query = _unitOfWork.Accounts.GetQueryable()
                .Include(a => a.Role)
                .Include(a => a.Status)
                .Include(a => a.UserProfile)
                .ThenInclude(up => up.SkillLevel)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => 
                    a.Email.Contains(searchTerm) || 
                    (a.UserProfile != null && (a.UserProfile.FirstName.Contains(searchTerm) || a.UserProfile.LastName.Contains(searchTerm))));
            }

            if (roleId.HasValue)
                query = query.Where(a => a.RoleId == roleId.Value);

            if (isActive.HasValue)
                query = query.Where(a => a.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<AccountDto>
            {
                Data = items.Select(MapToDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all accounts");
            throw;
        }
    }

    public async Task<AccountDto?> GetByIdAsync(int id)
    {
        try
        {
            var account = await _unitOfWork.Accounts.GetQueryable()
                .Include(a => a.Role)
                .Include(a => a.Status)
                .Include(a => a.UserProfile)
                .ThenInclude(up => up.SkillLevel)
                .FirstOrDefaultAsync(a => a.Id == id);

            return account != null ? MapToDto(account) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account with ID: {AccountId}", id);
            throw;
        }
    }

    public async Task<AccountDto?> GetByEmailAsync(string email)
    {
        try
        {
            var account = await _unitOfWork.Accounts.GetQueryable()
                .Include(a => a.Role)
                .Include(a => a.Status)
                .Include(a => a.UserProfile)
                .ThenInclude(up => up.SkillLevel)
                .FirstOrDefaultAsync(a => a.Email == email);

            return account != null ? MapToDto(account) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by email: {Email}", email);
            throw;
        }
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto accountDto)
    {
        try
        {
            var account = new Account
            {
                Email = accountDto.Email,
                Password = HashPassword(accountDto.Password ?? "defaultpassword"), 
                IsActive = accountDto.IsActive,
                Image = accountDto.Image,
                RoleId = accountDto.RoleId,
                StatusId = accountDto.StatusId
            };

            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(account.Id)
               ?? throw new Exception("Error retrieving newly created account.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            throw;
        }
    }

    public async Task<AccountDto?> UpdateAsync(int id, UpdateAccountDto updateDto)
    {
        try
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(id);
            if (account == null)
                return null;

            if (updateDto.IsActive.HasValue)
                account.IsActive = updateDto.IsActive.Value;
            if (updateDto.Image != null)
                account.Image = updateDto.Image;

            account.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Accounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account with ID: {AccountId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(id);
            if (account == null)
                return false;

            account.IsActive = false;
            account.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Accounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account with ID: {AccountId}", id);
            throw;
        }
    }

    public async Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword)
    {
        try
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(id);
            if (account == null)
                return false;

            // Verify current password
            if (!VerifyPassword(currentPassword, account.Password))
                return false;

            // Hash and update new password
            account.Password = HashPassword(newPassword);
            account.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Accounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for account with ID: {AccountId}", id);
            throw;
        }
    }

    public async Task<AccountDto?> SetActiveStatusAsync(int id, bool isActive)
    {
        try
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(id);
            if (account == null)
                return null;

            account.IsActive = isActive;
            account.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Accounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting active status for account with ID: {AccountId}", id);
            throw;
        }
    }

    public async Task<AccountDto?> ChangeRoleAsync(int id, int roleId)
    {
        try
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(id);
            if (account == null)
                return null;

            account.RoleId = roleId;
            account.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Accounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing role for account with ID: {AccountId}", id);
            throw;
        }
    }

    #region Helper Methods

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }

    private AccountDto MapToDto(Account account)
    {
        return new AccountDto
        {
            Id = account.Id,
            Email = account.Email,
            IsActive = account.IsActive,
            Image = account.Image,
            RoleId = account.RoleId,
            RoleTitle = account.Role?.Title,
            StatusId = account.StatusId,
            StatusValue = account.Status?.Value,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt.Date,
            UserProfile = account.UserProfile != null ? new UserProfileDto
            {
                Id = account.UserProfile.Id,
                FirstName = account.UserProfile.FirstName,
                LastName = account.UserProfile.LastName,
                DateOfBirth = account.UserProfile.DateOfBirth,
                Experience = account.UserProfile.Experience,
                AccountId = account.UserProfile.AccountId,
                SkillLevelId = account.UserProfile.SkillLevelId,
                SkillLevelValue = account.UserProfile.SkillLevel?.Title
            } : null
        };
    }

    #endregion
}
