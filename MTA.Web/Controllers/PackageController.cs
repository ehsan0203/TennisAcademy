using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using Microsoft.Extensions.Logging;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class PackageController : ControllerBase
{
    private readonly IPackageService _packageService;
    private readonly ILogger<PackageController> _logger;

    public PackageController(IPackageService packageService, ILogger<PackageController> logger)
    {
        _packageService = packageService;
        _logger = logger;
    }

    #region CRUD Operations

    /// <summary>
    /// Get all packages with pagination and filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PackageDto>>> GetPackages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? durationUnitId = null)
    {
        try
        {
            var result = await _packageService.GetAllAsync(page, pageSize, searchTerm, minPrice, maxPrice, durationUnitId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting packages");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get package by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PackageDto>> GetPackage(int id)
    {
        try
        {
            var package = await _packageService.GetByIdAsync(id);
            if (package == null)
                return NotFound("Package not found");

            return Ok(package);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting package with ID: {PackageId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get packages by price range
    /// </summary>
    [HttpGet("by-price-range")]
    public async Task<ActionResult<IEnumerable<PackageDto>>> GetPackagesByPriceRange(
        [FromQuery] decimal minPrice,
        [FromQuery] decimal maxPrice)
    {
        try
        {
            var packages = await _packageService.GetByPriceRangeAsync(minPrice, maxPrice);
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting packages by price range from {MinPrice} to {MaxPrice}", minPrice, maxPrice);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get packages by duration unit
    /// </summary>
    [HttpGet("by-duration-unit/{durationUnitId}")]
    public async Task<ActionResult<IEnumerable<PackageDto>>> GetPackagesByDurationUnit(int durationUnitId)
    {
        try
        {
            var packages = await _packageService.GetByDurationUnitAsync(durationUnitId);
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting packages by duration unit ID: {DurationUnitId}", durationUnitId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create new package
    /// </summary>
    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> CreatePackage([FromBody] PackageDto packageDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdPackage = await _packageService.CreateAsync(packageDto);
            return CreatedAtAction(nameof(GetPackage), new { id = createdPackage.Id }, createdPackage);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating package");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update existing package
    /// </summary>
    [HttpPut("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> UpdatePackage(int id, [FromBody] PackageDto packageDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedPackage = await _packageService.UpdateAsync(id, packageDto);
            if (updatedPackage == null)
                return NotFound("Package not found");

            return Ok(updatedPackage);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating package with ID: {PackageId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete package
    /// </summary>
    [HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeletePackage(int id)
    {
        try
        {
            var result = await _packageService.DeleteAsync(id);
            if (!result)
                return NotFound("Package not found");

            return Ok("Package deleted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting package with ID: {PackageId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion

    #region Advanced Operations

    /// <summary>
    /// Update package price
    /// </summary>
    [HttpPatch("{id}/price")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> UpdatePrice(int id, [FromBody] decimal price)
    {
        try
        {
            var updatedPackage = await _packageService.UpdatePriceAsync(id, price);
            return Ok(updatedPackage);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating price for package with ID: {PackageId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update package capacity
    /// </summary>
    [HttpPatch("{id}/capacity")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> UpdateCapacity(int id, [FromBody] UpdateCapacityDto capacityDto)
    {
        try
        {
            var updatedPackage = await _packageService.UpdateCapacityAsync(id, capacityDto.TicketCount, capacityDto.MessageCount);
            return Ok(updatedPackage);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating capacity for package with ID: {PackageId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update package duration
    /// </summary>
    [HttpPatch("{id}/duration")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> UpdateDuration(int id, [FromBody] UpdateDurationDto durationDto)
    {
        try
        {
            var updatedPackage = await _packageService.UpdateDurationAsync(id, durationDto.Duration, durationDto.DurationUnitId);
            return Ok(updatedPackage);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating duration for package with ID: {PackageId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion


    #region Search and Filter

    /// <summary>
    /// Search packages by title
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<PaginatedResult<PackageDto>>> SearchPackages([FromBody] PackageSearchDto searchDto)
    {
        try
        {
            var result = await _packageService.GetAllAsync(
                searchDto.Page, 
                searchDto.PageSize, 
                searchDto.SearchTerm,
                searchDto.MinPrice,
                searchDto.MaxPrice,
                searchDto.DurationUnitId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching packages");
            return StatusCode(500, "Internal server error");
        }
    }

    #endregion
}


