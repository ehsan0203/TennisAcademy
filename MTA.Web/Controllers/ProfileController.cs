using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs.User;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IAccountService _accountService;

    public ProfileController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(CustomJsonResult<CurrentUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<CurrentUserDto>> GetCurrentUser(CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirst("UserId")?.Value, out var userId) || userId == 0)
            return CustomJsonResult<CurrentUserDto>.Unauthorized("User is not authenticated.");

        var user = await _accountService.GetCurrentUserAsync(userId, ct);
        return user is null
            ? CustomJsonResult<CurrentUserDto>.NotFound("User not found.")
            : CustomJsonResult<CurrentUserDto>.SuccessResult(user);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(CustomJsonResult<CurrentUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<CurrentUserDto>> UpdateCurrentUser([FromForm] UpdateCurrentUserDto updateDto, CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirst("UserId")?.Value, out var userId) || userId == 0)
            return CustomJsonResult<CurrentUserDto>.Unauthorized("User is not authenticated.");

        var updated = await _accountService.UpdateCurrentUserAsync(userId, updateDto, ct);
        return updated is null
            ? CustomJsonResult<CurrentUserDto>.NotFound("User not found.")
            : CustomJsonResult<CurrentUserDto>.SuccessResult(updated);
    }

    [HttpPost("me/avatar")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> UploadAvatar([FromForm] IFormFile profileImage, CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirst("UserId")?.Value, out var userId) || userId == 0)
            return CustomJsonResult<string>.Unauthorized("User is not authenticated.");

        var path = await _accountService.UploadProfileImageAsync(userId, profileImage, ct);
        return path is null
            ? CustomJsonResult<string>.NotFound("User not found.")
            : CustomJsonResult<string>.SuccessResult(path);
    }
}
