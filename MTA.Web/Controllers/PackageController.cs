using System;
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
        [FromQuery] DateTime? expiresAfter = null,
        [FromQuery] DateTime? expiresBefore = null)
    {
        try
        {
            var result = await _packageService.GetAllAsync(page, pageSize, searchTerm, minPrice, maxPrice, expiresAfter, expiresBefore);
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

    ///// <summary>
    ///// Get packages by price range
    ///// </summary>
    //[HttpGet("by-price-range")]
    //public async Task<ActionResult<IEnumerable<PackageDto>>> GetPackagesByPriceRange(
    //    [FromQuery] decimal minPrice,
    //    [FromQuery] decimal maxPrice)
    //{
    //    try
    //    {
    //        var packages = await _packageService.GetByPriceRangeAsync(minPrice, maxPrice);
    //        return Ok(packages);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error getting packages by price range from {MinPrice} to {MaxPrice}", minPrice, maxPrice);
    //        return StatusCode(500, "Internal server error");
    //    }
    //}

    /// <summary>
    /// Create new package
    /// </summary>
    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<PackageDto>> CreatePackage([FromBody] CreatePackageDto packageDto)
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

    ///// <summary>
    ///// Update package price
    ///// </summary>
    //[HttpPatch("{id}/price")]
    ////[Authorize(Roles = "Admin")]
    //public async Task<ActionResult<PackageDto>> UpdatePrice(int id, [FromBody] decimal price)
    //{
    //    try
    //    {
    //        var updatedPackage = await _packageService.UpdatePriceAsync(id, price);
    //        return Ok(updatedPackage);
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error updating price for package with ID: {PackageId}", id);
    //        return StatusCode(500, "Internal server error");
    //    }
    //}

    ///// <summary>
    ///// Update package credits
    ///// </summary>
    //[HttpPatch("{id}/credits")]
    ////[Authorize(Roles = "Admin")]
    //public async Task<ActionResult<PackageDto>> UpdateCredits(int id, [FromBody] UpdateCreditsDto creditsDto)
    //{
    //    try
    //    {
    //        var updatedPackage = await _packageService.UpdateCreditsAsync(id, creditsDto.CreditCount);
    //        return Ok(updatedPackage);
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error updating credits for package with ID: {PackageId}", id);
    //        return StatusCode(500, "Internal server error");
    //    }
    //}

    ///// <summary>
    ///// Update package expiration date
    ///// </summary>
    //[HttpPatch("{id}/expiration")]
    ////[Authorize(Roles = "Admin")]
    //public async Task<ActionResult<PackageDto>> UpdateExpiration(int id, [FromBody] DateTime expirationDate)
    //{
    //    try
    //    {
    //        var updatedPackage = await _packageService.UpdateExpirationAsync(id, expirationDate);
    //        return Ok(updatedPackage);
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error updating expiration for package with ID: {PackageId}", id);
    //        return StatusCode(500, "Internal server error");
    //    }
    //}

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
                searchDto.ExpiresAfter,
                searchDto.ExpiresBefore);
            
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


