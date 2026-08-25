using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LookupController : ControllerBase
{
    private readonly ILookupService _lookupService;

    public LookupController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<LookupDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<LookupDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? category = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        var result = await _lookupService.GetAllAsync(page, pageSize, category, searchTerm, ct);
        return CustomJsonResult<PaginatedResult<LookupDto>>.SuccessResult(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<LookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<LookupDto>> GetById(int id, CancellationToken ct)
    {
        var lookup = await _lookupService.GetByIdAsync(id, ct);
        return lookup is null
            ? CustomJsonResult<LookupDto>.NotFound($"Lookup with ID {id} not found")
            : CustomJsonResult<LookupDto>.SuccessResult(lookup);
    }

    [HttpGet("category/{category}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<LookupDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<LookupDto>>> GetByCategory(string category, CancellationToken ct)
    {
        var result = await _lookupService.GetByCategoryAsync(category, ct);
        return CustomJsonResult<IEnumerable<LookupDto>>.SuccessResult(result);
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<string>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<string>>> GetCategories(CancellationToken ct)
    {
        var result = await _lookupService.GetAllCategoriesAsync(ct);
        return CustomJsonResult<IEnumerable<string>>.SuccessResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<LookupDto>), StatusCodes.Status201Created)]
    public async Task<CustomJsonResult<LookupDto>> Create([FromBody] CreateLookupDto lookupDto, CancellationToken ct)
    {
        var created = await _lookupService.CreateAsync(lookupDto, ct);
        return CustomJsonResult<LookupDto>.Created(created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<LookupDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<LookupDto>> Update(int id, [FromBody] LookupDto lookupDto, CancellationToken ct)
    {
        var updated = await _lookupService.UpdateAsync(id, lookupDto, ct);
        return CustomJsonResult<LookupDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> Delete(int id, CancellationToken ct)
    {
        var deleted = await _lookupService.DeleteAsync(id, ct);
        return deleted
            ? CustomJsonResult<string>.NoContent()
            : CustomJsonResult<string>.NotFound($"Lookup with ID {id} not found");
    }
}
