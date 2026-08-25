using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SiteRulesController : ControllerBase
{
    private readonly ILookupService _lookupService;

    public SiteRulesController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<PaginatedResult<LookupDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaginatedResult<LookupDto>>> GetSiteRules(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _lookupService.GetAllAsync(page, pageSize, "SiteRule", ct: ct);
        return CustomJsonResult<PaginatedResult<LookupDto>>.SuccessResult(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<LookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<LookupDto>> GetById(int id, CancellationToken ct)
    {
        var rule = await _lookupService.GetByIdAsync(id, ct);
        if (rule == null || rule.Category != "SiteRule")
            return CustomJsonResult<LookupDto>.NotFound($"Site rule with ID {id} not found");
        return CustomJsonResult<LookupDto>.SuccessResult(rule);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomJsonResult<LookupDto>), StatusCodes.Status201Created)]
    public async Task<CustomJsonResult<LookupDto>> Create([FromBody] SiteRuleUpsertDto dto, CancellationToken ct)
    {
        var createDto = new CreateLookupDto { Category = "SiteRule", Key = dto.Subject, Value = dto.Text };
        var created = await _lookupService.CreateAsync(createDto, ct);
        return CustomJsonResult<LookupDto>.Created(created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<LookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<LookupDto>> Update(int id, [FromBody] SiteRuleUpsertDto dto, CancellationToken ct)
    {
        var existing = await _lookupService.GetByIdAsync(id, ct);
        if (existing == null || existing.Category != "SiteRule")
            return CustomJsonResult<LookupDto>.NotFound($"Site rule with ID {id} not found");

        var updateDto = new LookupDto { Id = id, Category = "SiteRule", Key = dto.Subject, Value = dto.Text };
        var updated = await _lookupService.UpdateAsync(id, updateDto, ct);
        return CustomJsonResult<LookupDto>.SuccessResult(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> Delete(int id, CancellationToken ct)
    {
        var existing = await _lookupService.GetByIdAsync(id, ct);
        if (existing == null || existing.Category != "SiteRule")
            return CustomJsonResult<string>.NotFound($"Site rule with ID {id} not found");

        await _lookupService.DeleteAsync(id, ct);
        return CustomJsonResult<string>.NoContent();
    }
}
