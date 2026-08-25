using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("system")]
    [ProducesResponseType(typeof(CustomJsonResult<SystemStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<SystemStatisticsDto>> GetSystemStatistics(CancellationToken ct)
    {
        var stats = await _statisticsService.GetSystemStatisticsAsync(ct);
        return CustomJsonResult<SystemStatisticsDto>.SuccessResult(stats);
    }
}
