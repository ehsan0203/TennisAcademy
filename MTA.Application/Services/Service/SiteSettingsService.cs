using Microsoft.AspNetCore.Http;
using MTA.Application.DTOs;
using MTA.Application.Services.Interface;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services.Service;

public class SiteSettingsService : ISiteSettingsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public SiteSettingsService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<IEnumerable<SiteImageDto>> GetSiteImagesAsync(CancellationToken ct = default)
    {
        var existing = await _unitOfWork.Repository<SiteImage>().GetAllAsync(ct);
        var byKey = existing.ToDictionary(i => i.Key, i => i.Url);

        return SiteImageKeys.All.Select(key => new SiteImageDto
        {
            Key = key,
            Url = byKey.TryGetValue(key, out var url) ? url : null
        });
    }

    public async Task<SiteImageDto> SetSiteImageAsync(string key, IFormFile file, CancellationToken ct = default)
    {
        if (!SiteImageKeys.All.Contains(key))
            throw new ArgumentException($"Unknown site image key: {key}");

        var repo = _unitOfWork.Repository<SiteImage>();
        var existing = await repo.FirstOrDefaultAsync(i => i.Key == key, ct);
        var oldUrl = existing?.Url;

        var newUrl = await _fileStorageService.SaveFileAsync(file, "SiteImages", key);

        if (existing != null)
        {
            existing.Url = newUrl;
            await repo.UpdateAsync(existing, ct);
        }
        else
        {
            await repo.AddAsync(new SiteImage { Key = key, Url = newUrl }, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(oldUrl))
            await _fileStorageService.DeleteFileAsync(oldUrl);

        return new SiteImageDto { Key = key, Url = newUrl };
    }

    public async Task<IEnumerable<FooterContactItemDto>> GetFooterContactItemsAsync(CancellationToken ct = default)
    {
        var items = await _unitOfWork.Repository<FooterContactItem>().GetAllAsync(ct);
        return items
            .OrderBy(i => i.SortOrder)
            .Select(MapToDto);
    }

    public async Task<FooterContactItemDto> CreateFooterContactItemAsync(UpsertFooterContactItemDto dto, CancellationToken ct = default)
    {
        var entity = new FooterContactItem
        {
            Label = dto.Label,
            Value = dto.Value,
            SortOrder = dto.SortOrder
        };

        var created = await _unitOfWork.Repository<FooterContactItem>().AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return MapToDto(created);
    }

    public async Task<FooterContactItemDto> UpdateFooterContactItemAsync(int id, UpsertFooterContactItemDto dto, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<FooterContactItem>();
        var existing = await repo.GetByIdAsync(id, ct)
            ?? throw new ArgumentException($"Footer contact item with ID {id} not found");

        existing.Label = dto.Label;
        existing.Value = dto.Value;
        existing.SortOrder = dto.SortOrder;

        var updated = await repo.UpdateAsync(existing, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return MapToDto(updated);
    }

    public async Task DeleteFooterContactItemAsync(int id, CancellationToken ct = default)
    {
        var deleted = await _unitOfWork.Repository<FooterContactItem>().DeleteAsync(id, ct);
        if (!deleted)
            throw new ArgumentException($"Footer contact item with ID {id} not found");

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static FooterContactItemDto MapToDto(FooterContactItem item) => new()
    {
        Id = item.Id,
        Label = item.Label,
        Value = item.Value,
        SortOrder = item.SortOrder
    };
}
