using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Attributes;

namespace MTA.Web.Controllers;

/// <summary>
/// Controller for managing package history
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class PackageHistoryController : ControllerBase
{
    private readonly IPackageHistoryService _packageHistoryService;

    public PackageHistoryController(IPackageHistoryService packageHistoryService)
    {
        _packageHistoryService = packageHistoryService;
    }

    #region PackageHistory CRUD Operations

    /// <summary>
    /// Gets all package histories with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="accountId">Filter by account ID</param>
    /// <param name="packageId">Filter by package ID</param>
    /// <param name="isExpired">Filter by expired status</param>
    /// <returns>Paginated list of package histories</returns>
    /// <response code="200">Returns the list of package histories</response>
    /// <response code="400">If the filter parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<PackageHistoryDto>>> GetPackageHistories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? accountId = null,
        [FromQuery] int? packageId = null,
        [FromQuery] bool? isExpired = null)
    {
        try
        {
            var result = await _packageHistoryService.GetAllAsync(page, pageSize, accountId, packageId, isExpired);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a specific package history by ID
    /// </summary>
    /// <param name="id">The ID of the package history</param>
    /// <returns>The requested package history</returns>
    /// <response code="200">Returns the requested package history</response>
    /// <response code="404">If the package history was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PackageHistoryDto>> GetPackageHistory(int id)
    {
        try
        {
            var packageHistory = await _packageHistoryService.GetByIdAsync(id);
            if (packageHistory == null)
                return NotFound($"Package history with ID {id} not found");

            return Ok(packageHistory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new package history
    /// </summary>
    /// <param name="packageHistoryDto">The package history data</param>
    /// <returns>The created package history</returns>
    /// <response code="201">Returns the newly created package history</response>
    /// <response code="400">If the package history data is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PackageHistoryDto>> CreatePackageHistory([FromBody] PackageHistoryDto packageHistoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdPackageHistory = await _packageHistoryService.CreateAsync(packageHistoryDto);
            return CreatedAtAction(nameof(GetPackageHistory), new { id = createdPackageHistory.Id }, createdPackageHistory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing package history
    /// </summary>
    /// <param name="id">The ID of the package history to update</param>
    /// <param name="packageHistoryDto">The updated package history data</param>
    /// <returns>The updated package history</returns>
    /// <response code="200">Returns the updated package history</response>
    /// <response code="400">If the package history data is invalid</response>
    /// <response code="404">If the package history was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PackageHistoryDto>> UpdatePackageHistory(int id, [FromBody] PackageHistoryDto packageHistoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedPackageHistory = await _packageHistoryService.UpdateAsync(id, packageHistoryDto);
            return Ok(updatedPackageHistory);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a package history
    /// </summary>
    /// <param name="id">The ID of the package history to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the package history was deleted successfully</response>
    /// <response code="404">If the package history was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePackageHistory(int id)
    {
        try
        {
            var deleted = await _packageHistoryService.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Package history with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region PackageHistory Update Operations

    /// <summary>
    /// Updates the remaining tickets for a package history
    /// </summary>
    /// <param name="id">The ID of the package history</param>
    /// <param name="remainingTickets">The new remaining tickets count</param>
    /// <returns>The updated package history</returns>
    /// <response code="200">Returns the updated package history</response>
    /// <response code="404">If the package history was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/remaining-tickets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PackageHistoryDto>> UpdateRemainingTickets(int id, [FromBody] int remainingTickets)
    {
        try
        {
            if (remainingTickets < 0)
                return BadRequest("Remaining tickets cannot be negative");

            var updatedPackageHistory = await _packageHistoryService.UpdateRemainingTicketsAsync(id, remainingTickets);
            return Ok(updatedPackageHistory);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the remaining messages for a package history
    /// </summary>
    /// <param name="id">The ID of the package history</param>
    /// <param name="remainingMessages">The new remaining messages count</param>
    /// <returns>The updated package history</returns>
    /// <response code="200">Returns the updated package history</response>
    /// <response code="404">If the package history was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/remaining-messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PackageHistoryDto>> UpdateRemainingMessages(int id, [FromBody] int remainingMessages)
    {
        try
        {
            if (remainingMessages < 0)
                return BadRequest("Remaining messages cannot be negative");

            var updatedPackageHistory = await _packageHistoryService.UpdateRemainingMessagesAsync(id, remainingMessages);
            return Ok(updatedPackageHistory);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Extends the expiration date for a package history
    /// </summary>
    /// <param name="id">The ID of the package history</param>
    /// <param name="newExpiryDate">The new expiration date</param>
    /// <returns>The updated package history</returns>
    /// <response code="200">Returns the updated package history</response>
    /// <response code="400">If the new expiry date is invalid</response>
    /// <response code="404">If the package history was not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPatch("{id}/extend-expiration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PackageHistoryDto>> ExtendExpiration(int id, [FromBody] DateTime newExpiryDate)
    {
        try
        {
            if (newExpiryDate <= DateTime.UtcNow)
                return BadRequest("New expiry date must be in the future");

            var updatedPackageHistory = await _packageHistoryService.ExtendExpirationAsync(id, newExpiryDate);
            return Ok(updatedPackageHistory);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region PackageHistory Queries

    /// <summary>
    /// Gets package histories by account ID
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>List of package histories for the account</returns>
    /// <response code="200">Returns the list of package histories</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("account/{accountId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PackageHistoryDto>>> GetPackageHistoriesByAccount(int accountId)
    {
        try
        {
            var packageHistories = await _packageHistoryService.GetByAccountAsync(accountId);
            return Ok(packageHistories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets package histories by package ID
    /// </summary>
    /// <param name="packageId">The ID of the package</param>
    /// <returns>List of package histories for the package</returns>
    /// <response code="200">Returns the list of package histories</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("package/{packageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PackageHistoryDto>>> GetPackageHistoriesByPackage(int packageId)
    {
        try
        {
            var packageHistories = await _packageHistoryService.GetByPackageAsync(packageId);
            return Ok(packageHistories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets active package histories for an account
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>List of active package histories</returns>
    /// <response code="200">Returns the list of active package histories</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("account/{accountId}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PackageHistoryDto>>> GetActivePackageHistories(int accountId)
    {
        try
        {
            var packageHistories = await _packageHistoryService.GetActiveByAccountAsync(accountId);
            return Ok(packageHistories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets expired package histories for an account
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>List of expired package histories</returns>
    /// <response code="200">Returns the list of expired package histories</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("account/{accountId}/expired")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PackageHistoryDto>>> GetExpiredPackageHistories(int accountId)
    {
        try
        {
            var packageHistories = await _packageHistoryService.GetExpiredByAccountAsync(accountId);
            return Ok(packageHistories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if a user has an active package
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="packageId">The ID of the package</param>
    /// <returns>True if user has active package</returns>
    /// <response code="200">Returns the result</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("check-active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> CheckUserHasActivePackage(
        [FromQuery] int accountId,
        [FromQuery] int packageId)
    {
        try
        {
            var hasActivePackage = await _packageHistoryService.UserHasActivePackageAsync(accountId, packageId);
            return Ok(hasActivePackage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets package histories by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of package histories in the date range</returns>
    /// <response code="200">Returns the list of package histories</response>
    /// <response code="400">If the date parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("date-range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PackageHistoryDto>>> GetPackageHistoriesByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
                return BadRequest("Start date cannot be after end date");

            var packageHistories = await _packageHistoryService.GetByDateRangeAsync(startDate, endDate);
            return Ok(packageHistories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets expiring packages (expiring within specified days)
    /// </summary>
    /// <param name="days">Number of days (default: 7)</param>
    /// <returns>List of expiring packages</returns>
    /// <response code="200">Returns the list of expiring packages</response>
    /// <response code="400">If the days parameter is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("expiring")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PackageHistoryDto>>> GetExpiringPackages([FromQuery] int days = 7)
    {
        try
        {
            if (days <= 0)
                return BadRequest("Days must be greater than 0");

            var expiringPackages = await _packageHistoryService.GetExpiringPackagesAsync(days);
            return Ok(expiringPackages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion

    #region PackageHistory Statistics and Reports

    /// <summary>
    /// Gets package history statistics
    /// </summary>
    /// <returns>Package history statistics</returns>
    /// <response code="200">Returns the package history statistics</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PackageHistoryStatisticsDto>> GetPackageHistoryStatistics()
    {
        try
        {
            var statistics = await _packageHistoryService.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets user package usage summary
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>User package usage summary</returns>
    /// <response code="200">Returns the user package usage summary</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("account/{accountId}/usage-summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserPackageUsageSummaryDto>> GetUserPackageUsageSummary(int accountId)
    {
        try
        {
            var usageSummary = await _packageHistoryService.GetUserPackageUsageSummaryAsync(accountId);
            return Ok(usageSummary);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    #endregion
}
