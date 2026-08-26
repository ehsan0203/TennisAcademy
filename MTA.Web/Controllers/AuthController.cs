using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs.Auth;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status401Unauthorized)]
    public async Task<CustomJsonResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto, CancellationToken ct)
    {
        var response = await _authService.LoginAsync(loginDto, ct);
        return CustomJsonResult<AuthResponseDto>.SuccessResult(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status409Conflict)]
    public async Task<CustomJsonResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto, CancellationToken ct)
    {
        var response = await _authService.RegisterAsync(registerDto, ct);
        return CustomJsonResult<AuthResponseDto>.Created(response);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status401Unauthorized)]
    public async Task<CustomJsonResult<AuthResponseDto>> RefreshToken([FromBody] string refreshToken, CancellationToken ct)
    {
        var response = await _authService.RefreshTokenAsync(refreshToken, ct);
        return CustomJsonResult<AuthResponseDto>.SuccessResult(response);
    }

    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> RevokeToken([FromBody] string refreshToken, CancellationToken ct)
    {
        var result = await _authService.RevokeTokenAsync(refreshToken, ct);
        return result
            ? CustomJsonResult<string>.SuccessResult("Token revoked successfully")
            : CustomJsonResult<string>.NotFound("Refresh token not found");
    }

    [HttpPost("validate-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<bool>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<bool>> ValidateToken([FromBody] string token, CancellationToken ct)
    {
        var isValid = await _authService.ValidateTokenAsync(token, ct);
        return CustomJsonResult<bool>.SuccessResult(isValid);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<string>> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken ct)
    {
        await _authService.ForgotPasswordAsync(dto, ct);
        return CustomJsonResult<string>.SuccessResult("If an account with that email exists, a reset link has been sent.");
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<string>> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken ct)
    {
        await _authService.ResetPasswordAsync(dto, ct);
        return CustomJsonResult<string>.SuccessResult("Password reset successfully.");
    }
}
