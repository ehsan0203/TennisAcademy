using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.DTOs.Auth;
using MTA.Application.Services;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILookupService _lookupService;

    public AuthController(IAuthService authService, ILookupService lookupService)
    {
        _authService = authService;
        _lookupService = lookupService;
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Authentication response with tokens</returns>
    /// <response code="200">Returns authentication tokens</response>
    /// <response code="400">If the login data is invalid</response>
    /// <response code="401">If the credentials are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerDto">Registration data</param>
    /// <returns>Authentication response with tokens</returns>
    /// <response code="201">Returns the newly created user with tokens</response>
    /// <response code="400">If the registration data is invalid</response>
    /// <response code="409">If the email already exists</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var response = await _authService.RegisterAsync(registerDto);
            return CreatedAtAction(nameof(Login), response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Email already exists"))
        {
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Default role not found"))
        {
            return StatusCode(500, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Refreshes an expired access token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New authentication response</returns>
    /// <response code="200">Returns new authentication tokens</response>
    /// <response code="400">If the refresh token is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var response = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(response);
        }
        catch (NotImplementedException)
        {
            return StatusCode(500, new { message = "Refresh token functionality not implemented yet" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <returns>Success message</returns>
    /// <response code="200">If the token was revoked successfully</response>
    /// <response code="400">If the refresh token is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("revoke-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RevokeToken([FromBody] string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var result = await _authService.RevokeTokenAsync(refreshToken);
            if (result)
            {
                return Ok(new { message = "Token revoked successfully" });
            }

            return StatusCode(500, new { message = "Failed to revoke token" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>Validation result</returns>
    /// <response code="200">Returns token validation result</response>
    /// <response code="400">If the token is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("validate-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ValidateToken([FromBody] string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var isValid = await _authService.ValidateTokenAsync(token);
            return Ok(new { isValid });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }
}
