using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.DTOs.User;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly IAccountService _accountService;

    public UserController(IUserProfileService userProfileService, IAccountService accountService)
    {
        _userProfileService = userProfileService;
        _accountService = accountService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<AccountDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<AccountDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? roleId = null,
        [FromQuery] bool? isActive = null,
        CancellationToken ct = default)
    {
        var result = await _accountService.GetAllAsync(page, pageSize, null, roleId, isActive, ct);
        return CustomJsonResult<PaginatedResult<AccountDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<AccountDto>> GetUser(int id, CancellationToken ct)
    {
        var account = await _accountService.GetByIdAsync(id, ct);
        return account is null
            ? CustomJsonResult<AccountDto>.NotFound("User not found")
            : CustomJsonResult<AccountDto>.SuccessResult(account);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<UserProfileDto>> UpdateUser(int id, [FromForm] UpdateUserProfileDto updateDto, CancellationToken ct)
    {
        var updated = await _userProfileService.UpdateAsync(id, updateDto, ct);
        return updated is null
            ? CustomJsonResult<UserProfileDto>.NotFound("User profile not found")
            : CustomJsonResult<UserProfileDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteUser(int id, CancellationToken ct)
    {
        var result = await _accountService.DeleteAsync(id, ct);
        return result
            ? CustomJsonResult<string>.SuccessResult("User deactivated successfully")
            : CustomJsonResult<string>.NotFound("User not found");
    }

    [HttpPatch("{id}/avatar")]
    [ProducesResponseType(typeof(CustomJsonResult<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<UserProfileDto>> UpdateAvatar(int id, [FromBody] string imageUrl, CancellationToken ct)
    {
        var updated = await _userProfileService.UpdateAvatarAsync(id, imageUrl, ct);
        return updated is null
            ? CustomJsonResult<UserProfileDto>.NotFound("User profile not found")
            : CustomJsonResult<UserProfileDto>.SuccessResult(updated);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(CustomJsonResult<PagedResult<UserProfileDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PagedResult<UserProfileDto>>> QueryUsers([FromQuery] UserSearchDto queryDto, CancellationToken ct)
    {
        var result = await _userProfileService.QueryAsync(queryDto, ct);
        return CustomJsonResult<PagedResult<UserProfileDto>>.SuccessResult(result);
    }
}
