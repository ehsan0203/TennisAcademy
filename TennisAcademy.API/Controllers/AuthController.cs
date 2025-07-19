using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BuildingBlocks.Response;
using System.Net;
using TennisAcademy.Application.DTOs.Auth;
using TennisAcademy.Application.Interfaces.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(CustomJsonResult<AuthResultDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return new CustomJsonResult<AuthResultDto>(result);
    }
    [HttpPost("login")]
    [ProducesResponseType(typeof(CustomJsonResult<AuthResultDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return new CustomJsonResult<AuthResultDto>(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(CustomJsonResult<AuthResultDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.Token, dto.RefreshToken);
        if (result == null)
            return new CustomJsonResult<string>(null, StatusCodes.Status401Unauthorized);
        return new CustomJsonResult<AuthResultDto>(result);
    }

}
