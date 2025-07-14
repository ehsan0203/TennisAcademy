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
    [ProducesResponseType(typeof(CustomJsonResult<bool>), (int) HttpStatusCode.OK),
     ProducesResponseType(typeof(CustomJsonResult<string>), (int) HttpStatusCode.NotFound),
     ProducesResponseType(typeof(CustomJsonResult<string>), (int) HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        return Ok(result);
        return Ok(new CustomJsonResult<bool>(true));
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

}
