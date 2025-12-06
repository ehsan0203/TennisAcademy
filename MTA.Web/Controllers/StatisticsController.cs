using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(IUnitOfWork unitOfWork, ILogger<StatisticsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet("system")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SystemStatisticsDto>> GetSystemStatistics()
    {
        try
        {
            var pendingStatusId = await _unitOfWork.Repository<Lookup>()
                .GetQueryable()
                .Where(l => l.Category == "TicketStatus" && l.Key == "Pending")
                .Select(l => (int?)l.Id)
                .FirstOrDefaultAsync();

            var stats = new SystemStatisticsDto
            {
                RegisteredUsers = await _unitOfWork.Repository<Account>().CountAsync(),
                PurchasedCourses = await _unitOfWork.Repository<UserCourseHistory>().CountAsync(),
                PurchasedPackages = await _unitOfWork.Repository<PackageHistory>().CountAsync(),
                PendingTickets = pendingStatusId.HasValue
                    ? await _unitOfWork.Repository<Ticket>().CountAsync(t => t.StatusId == pendingStatusId.Value)
                    : 0
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load system statistics");
        }
    }
}
