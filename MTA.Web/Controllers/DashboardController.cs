using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpPost("[action]")]
    [ProducesResponseType(typeof(CustomJsonResult<DashboardStatisticsDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<CustomJsonResult<DashboardStatisticsDto>>> GetStatistics([FromQuery] int topCount = 5)
    {
        try
        {
            var statistics = await _dashboardService.GetStatisticsAsync(topCount);
            return Ok(CustomJsonResult<DashboardStatisticsDto>.SuccessResult(statistics));
        }
        catch (ArgumentOutOfRangeException exception)
        {
            _logger.LogWarning(exception, "Invalid top count provided for dashboard statistics.");
            return BadRequest(CustomJsonResult<string>.Failure(exception.Message));
        }
        catch (KeyNotFoundException exception)
        {
            _logger.LogWarning(exception, "Required data not found while retrieving dashboard statistics.");
            return NotFound(CustomJsonResult<string>.Failure(exception.Message));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while retrieving dashboard statistics.");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                CustomJsonResult<string>.Failure("An unexpected error occurred while retrieving dashboard statistics."));
        }
    }
}
