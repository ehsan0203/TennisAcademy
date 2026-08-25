using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PackageController : ControllerBase
{
    private readonly IPackageService _packageService;

    public PackageController(IPackageService packageService)
    {
        _packageService = packageService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<PackageDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<PackageDto>>> GetPackages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? durationUnitId = null,
        CancellationToken ct = default)
    {
        var result = await _packageService.GetAllAsync(page, pageSize, searchTerm, minPrice, maxPrice, durationUnitId, ct);
        return CustomJsonResult<PaginatedResult<PackageDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<PackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<PackageDto>> GetPackage(int id, CancellationToken ct)
    {
        var package = await _packageService.GetByIdAsync(id, ct);
        return package is null
            ? CustomJsonResult<PackageDto>.NotFound("Package not found")
            : CustomJsonResult<PackageDto>.SuccessResult(package);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<PackageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<PackageDto>> CreatePackage([FromBody] CreatePackageDto packageDto, CancellationToken ct)
    {
        var created = await _packageService.CreateAsync(packageDto, ct);
        return CustomJsonResult<PackageDto>.Created(created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<PackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<PackageDto>> UpdatePackage(int id, [FromBody] PackageDto packageDto, CancellationToken ct)
    {
        var updated = await _packageService.UpdateAsync(id, packageDto, ct);
        return CustomJsonResult<PackageDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeletePackage(int id, CancellationToken ct)
    {
        var deleted = await _packageService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound("Package not found");
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<PackageDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<PackageDto>>> SearchPackages([FromBody] PackageSearchDto searchDto, CancellationToken ct)
    {
        var result = await _packageService.GetAllAsync(
            searchDto.Page, searchDto.PageSize, searchDto.SearchTerm,
            searchDto.MinPrice, searchDto.MaxPrice, searchDto.DurationUnitId, ct);
        return CustomJsonResult<PaginatedResult<PackageDto>>.SuccessResult(result);
    }
}
