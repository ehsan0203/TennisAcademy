using BuildingBlocks.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int) HttpStatusCode.OK),
     ProducesResponseType(typeof(CustomJsonResult<string>), (int) HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        return Ok(new CustomJsonResult("Your registration was successful."));

    }
    [HttpPost("login")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(new CustomJsonResult("Login was successful."));

    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.Token, dto.RefreshToken);
        if (result == null)
            return Unauthorized();
        return Ok(result);
    }

}
