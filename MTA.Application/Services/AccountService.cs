using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Application.DTOs.User;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for Account operations (light & optimized)
/// </summary>
public class AccountService : IAccountService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountService> _logger;
    private readonly IMediaFileService _mediaFileService;

    public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger, IMediaFileService mediaFileService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediaFileService = mediaFileService;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<AccountDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? roleId = null, bool? isActive = null)
    {
        try
        {
            var query = _unitOfWork.Accounts.GetQueryable()
                .Include(a => a.Role)
                .Include(a => a.Status)
                .Include(a => a.MediaFile)
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
                .Include(a => a.MediaFile)
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
                .Include(a => a.MediaFile)
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
        MediaFileDto? uploadedMedia = null;

        try
        {
            if (accountDto.Image != null)
            {
                var mediaDto = new MediaFileUploadDto
                {
                    MediaType = "Account",
                    PlacementName = "ProfileImage",
                    Title = $"{accountDto.Email} Profile Image"
                };

                uploadedMedia = await _mediaFileService.CreateAsync(accountDto.Image, mediaDto);
            }

            var account = _mapper.Map<Account>(accountDto);
            account.Password = HashPassword(accountDto.Password ?? "defaultpassword");

            if (uploadedMedia != null)
            {
                account.MediaFileId = uploadedMedia.Id; // یا MediaFileId
            }

            // 3️⃣ ذخیره Account در دیتابیس
            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            // 4️⃣ برگرداندن DTO
            return _mapper.Map<AccountDto>(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account for Email: {Email}", accountDto.Email);

            // 5️⃣ rollback فایل آپلود شده در صورت خطای دیتابیس
            if (uploadedMedia != null)
            {
                try
                {
                    await _mediaFileService.DeleteAsync(uploadedMedia.Id);
                }
                catch (Exception fileEx)
                {
                    _logger.LogError(fileEx, "Error rolling back uploaded media file: {FileId}", uploadedMedia.Id);
                }
            }

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
            if (!string.IsNullOrEmpty(updateDto.Email))
                account.Email = updateDto.Email;
            if (updateDto.RoleId.HasValue)
                account.RoleId = updateDto.RoleId.Value;
            if (updateDto.StatusId.HasValue)
                account.StatusId = updateDto.StatusId.Value;
            if (updateDto.MediaFileId.HasValue)
                account.MediaFileId = updateDto.MediaFileId.Value;

            if (!string.IsNullOrEmpty(updateDto.Password))
                account.Password = HashPassword(updateDto.Password);

            if (updateDto.Image != null && updateDto.Image.Length > 0)
            {
                var mediaDto = new MediaFileUploadDto
                {
                    MediaType = "Account",
                    PlacementName = "ProfileImage",
                    Title = $"{account.Email} Profile Image"
                };

                var uploadedMedia = await _mediaFileService.CreateAsync(updateDto.Image, mediaDto);

                account.MediaFileId = uploadedMedia.Id;
            }

            account.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Accounts.UpdateAsync(account);
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
            Image = account.MediaFile.Url,
            RoleId = account.RoleId,
            RoleTitle = account.Role?.Title,
            StatusId = account.StatusId,
            StatusValue = account.Status?.Value,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt.Date,
            MediaFileId = account.MediaFileId,
            MediaFileUrl = account.MediaFile != null ? account.MediaFile.Url : null,
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
