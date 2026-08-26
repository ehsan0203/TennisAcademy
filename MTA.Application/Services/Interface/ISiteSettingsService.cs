using Microsoft.AspNetCore.Http;
using MTA.Application.DTOs;

namespace MTA.Application.Services.Interface;

public interface ISiteSettingsService
{
    Task<IEnumerable<SiteImageDto>> GetSiteImagesAsync(CancellationToken ct = default);
    Task<SiteImageDto> SetSiteImageAsync(string key, IFormFile file, CancellationToken ct = default);

    Task<IEnumerable<FooterContactItemDto>> GetFooterContactItemsAsync(CancellationToken ct = default);
    Task<FooterContactItemDto> CreateFooterContactItemAsync(UpsertFooterContactItemDto dto, CancellationToken ct = default);
    Task<FooterContactItemDto> UpdateFooterContactItemAsync(int id, UpsertFooterContactItemDto dto, CancellationToken ct = default);
    Task DeleteFooterContactItemAsync(int id, CancellationToken ct = default);
}
