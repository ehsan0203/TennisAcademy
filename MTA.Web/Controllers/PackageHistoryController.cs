using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

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

    [HttpGet]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<PackageHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<PackageHistoryDto>>> GetPackageHistories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? accountId = null,
        [FromQuery] int? packageId = null,
        [FromQuery] bool? isExpired = null,
        CancellationToken ct = default)
    {
        var result = await _packageHistoryService.GetAllAsync(page, pageSize, accountId, packageId, isExpired, ct);
        return CustomJsonResult<PaginatedResult<PackageHistoryDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<PackageHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<PackageHistoryDto>> GetPackageHistory(int id, CancellationToken ct)
    {
        var result = await _packageHistoryService.GetByIdAsync(id, ct);
        return result is null
            ? CustomJsonResult<PackageHistoryDto>.NotFound($"Package history with ID {id} not found")
            : CustomJsonResult<PackageHistoryDto>.SuccessResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<PackageHistoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<PackageHistoryDto>> CreatePackageHistory([FromBody] CreatePackageHistoryDto packageHistoryDto, CancellationToken ct)
    {
        var created = await _packageHistoryService.CreateAsync(packageHistoryDto, ct);
        return CustomJsonResult<PackageHistoryDto>.Created(created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<PackageHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<PackageHistoryDto>> UpdatePackageHistory(int id, [FromBody] PackageHistoryDto packageHistoryDto, CancellationToken ct)
    {
        var updated = await _packageHistoryService.UpdateAsync(id, packageHistoryDto, ct);
        return CustomJsonResult<PackageHistoryDto>.SuccessResult(updated);
    }

    [HttpPatch("{id}/remaining-tickets")]
    [ProducesResponseType(typeof(CustomJsonResult<PackageHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<PackageHistoryDto>> UpdateRemainingTickets(int id, [FromBody] UpdateRemainingTicketsDto dto, CancellationToken ct)
    {
        var updated = await _packageHistoryService.UpdateRemainingTicketsAsync(id, Math.Max(0, dto.RemainingTickets), ct);
        return CustomJsonResult<PackageHistoryDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeletePackageHistory(int id, CancellationToken ct)
    {
        var deleted = await _packageHistoryService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Package history with ID {id} not found");
    }

    [HttpGet("account/{accountId}")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<PackageHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<PackageHistoryDto>>> GetPackageHistoriesByAccount(int accountId, CancellationToken ct)
    {
        var result = await _packageHistoryService.GetByAccountAsync(accountId, ct);
        return CustomJsonResult<IEnumerable<PackageHistoryDto>>.SuccessResult(result);
    }

    [HttpGet("package/{packageId}")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<PackageHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<PackageHistoryDto>>> GetPackageHistoriesByPackage(int packageId, CancellationToken ct)
    {
        var result = await _packageHistoryService.GetByPackageAsync(packageId, ct);
        return CustomJsonResult<IEnumerable<PackageHistoryDto>>.SuccessResult(result);
    }

    [HttpGet("account/{accountId}/active")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<PackageHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<PackageHistoryDto>>> GetActivePackageHistories(int accountId, CancellationToken ct)
    {
        var result = await _packageHistoryService.GetActiveByAccountAsync(accountId, ct);
        return CustomJsonResult<IEnumerable<PackageHistoryDto>>.SuccessResult(result);
    }

    [HttpGet("account/{accountId}/expired")]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<PackageHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<PackageHistoryDto>>> GetExpiredPackageHistories(int accountId, CancellationToken ct)
    {
        var result = await _packageHistoryService.GetExpiredByAccountAsync(accountId, ct);
        return CustomJsonResult<IEnumerable<PackageHistoryDto>>.SuccessResult(result);
    }
}
