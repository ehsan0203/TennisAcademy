using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.DTOs.User;
using MTA.Application.Services;
using MTA.Domain.Entities;
using System.Security.Claims;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly IAccountService _accountService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserProfileService userProfileService,
        IAccountService accountService,
        ILogger<UserController> logger)
    {
        _userProfileService = userProfileService;
        _accountService = accountService;
        _logger = logger;
    }

    #region CRUD Operations

    /// <summary>
    /// Get all users with pagination and simple filtering
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaginatedResult<AccountDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? roleId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int? skillLevelId = null)
    {
        try
        {
            var result = await _accountService.GetAllAsync(page, pageSize, null, roleId, isActive);
            
            // Filter by skill level if specified
            if (skillLevelId.HasValue)
            {
                var filteredData = result.Data.Where(account => 
                    account.UserProfile?.SkillLevelId == skillLevelId.Value).ToList();
                
                result.Data = filteredData;
                result.TotalCount = filteredData.Count;
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get user details by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetUser(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (userId.Value != id && !User.IsInRole("Admin"))
                return Forbid();

            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
                return NotFound("User not found");

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with ID: {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserProfileDto>> UpdateUser(int id, [FromBody] UserProfileDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Users can only update their own profile, admins can update any profile
            if (userId.Value != id && !User.IsInRole("Admin"))
                return Forbid();

            var updatedProfile = await _userProfileService.UpdateAsync(id, updateDto);
            if (updatedProfile == null)
                return NotFound("User profile not found");

            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete or deactivate user
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            var result = await _accountService.DeleteAsync(id);
            if (!result)
                return NotFound("User not found");

            return Ok("User deactivated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion

    #region Partial Updates

    /// <summary>
    /// Update user avatar
    /// </summary>
    [HttpPatch("{id}/avatar")]
    public async Task<ActionResult<UserProfileDto>> UpdateAvatar(int id, [FromBody] string ImageUrl)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Users can only update their own avatar, admins can update any avatar
            if (userId.Value != id && !User.IsInRole("Admin"))
                return Forbid();

            var updatedProfile = await _userProfileService.UpdateAvatarAsync(id, ImageUrl);
            if (updatedProfile == null)
                return NotFound("User profile not found");

            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating avatar for user with ID: {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update user skill level
    /// </summary>
    [HttpPatch("{id}/skill-level")]
    public async Task<ActionResult<UserProfileDto>> UpdateSkillLevel(int id, [FromBody] int SkillLevelId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Users can only update their own skill level, admins can update any skill level
            if (userId.Value != id && !User.IsInRole("Admin"))
                return Forbid();

            var updatedProfile = await _userProfileService.UpdateSkillLevelAsync(id, SkillLevelId);
            if (updatedProfile == null)
                return NotFound("User profile not found");

            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating skill level for user with ID: {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update user experience
    /// </summary>
    [HttpPatch("{id}/experience")]
    public async Task<ActionResult<UserProfileDto>> UpdateExperience(int id, [FromBody] int ExperienceId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Users can only update their own experience, admins can update any experience
            if (userId.Value != id && !User.IsInRole("Admin"))
                return Forbid();

            var updatedProfile = await _userProfileService.UpdateExperienceAsync(id, ExperienceId);
            if (updatedProfile == null)
                return NotFound("User profile not found");

            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating experience for user with ID: {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion

    #region Search & Filter

    /// <summary>
    /// Search users
    /// </summary>
    /// <summary>
    /// Search and filter users (Admin only)
    /// </summary>
    [HttpPost("query")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedResult<UserProfileDto>>> QueryUsers([FromBody] UserSearchDto queryDto)
    {
        try
        {
            var result = await _userProfileService.QueryAsync(queryDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying users");
            return StatusCode(500, "Internal server error");
        }
    }


    #endregion

    #region Helper Methods

    /// <summary>
    /// Debug endpoint to check JWT claims (remove in production)
    /// </summary>
    [HttpGet("debug/claims")]
    [Authorize]
    public ActionResult GetClaims()
    {
        var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();
        return Ok(new
        {
            UserId = GetCurrentUserId(),
            Claims = claims,
            IsAuthenticated = User.Identity?.IsAuthenticated,
            Name = User.Identity?.Name
        });
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("UserId")?.Value;

        if (int.TryParse(userIdClaim, out int userId))
            return userId;

        _logger.LogWarning("Available claims: {Claims}",
            string.Join(", ", User.Claims.Select(c => $"{c.Type}:{c.Value}")));

        return null;
    }


    #endregion
}
