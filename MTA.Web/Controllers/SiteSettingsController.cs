using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services.Interface;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class SiteSettingsController : ControllerBase
{
    private readonly ISiteSettingsService _siteSettingsService;

    public SiteSettingsController(ISiteSettingsService siteSettingsService)
    {
        _siteSettingsService = siteSettingsService;
    }

    [HttpGet("images")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<SiteImageDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<SiteImageDto>>> GetImages(CancellationToken ct)
    {
        var result = await _siteSettingsService.GetSiteImagesAsync(ct);
        return CustomJsonResult<IEnumerable<SiteImageDto>>.SuccessResult(result);
    }

    [HttpPut("images/{key}")]
    [ProducesResponseType(typeof(CustomJsonResult<SiteImageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<SiteImageDto>> SetImage(string key, [FromForm] IFormFile file, CancellationToken ct)
    {
        var result = await _siteSettingsService.SetSiteImageAsync(key, file, ct);
        return CustomJsonResult<SiteImageDto>.SuccessResult(result);
    }

    [HttpGet("footer-contacts")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomJsonResult<IEnumerable<FooterContactItemDto>>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<IEnumerable<FooterContactItemDto>>> GetFooterContacts(CancellationToken ct)
    {
        var result = await _siteSettingsService.GetFooterContactItemsAsync(ct);
        return CustomJsonResult<IEnumerable<FooterContactItemDto>>.SuccessResult(result);
    }

    [HttpPost("footer-contacts")]
    [ProducesResponseType(typeof(CustomJsonResult<FooterContactItemDto>), StatusCodes.Status201Created)]
    public async Task<CustomJsonResult<FooterContactItemDto>> CreateFooterContact([FromBody] UpsertFooterContactItemDto dto, CancellationToken ct)
    {
        var created = await _siteSettingsService.CreateFooterContactItemAsync(dto, ct);
        return CustomJsonResult<FooterContactItemDto>.Created(created);
    }

    [HttpPut("footer-contacts/{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<FooterContactItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<FooterContactItemDto>> UpdateFooterContact(int id, [FromBody] UpsertFooterContactItemDto dto, CancellationToken ct)
    {
        var updated = await _siteSettingsService.UpdateFooterContactItemAsync(id, dto, ct);
        return CustomJsonResult<FooterContactItemDto>.SuccessResult(updated);
    }

    [HttpDelete("footer-contacts/{id}")]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<string>> DeleteFooterContact(int id, CancellationToken ct)
    {
        await _siteSettingsService.DeleteFooterContactItemAsync(id, ct);
        return CustomJsonResult<string>.NoContent();
    }
}
