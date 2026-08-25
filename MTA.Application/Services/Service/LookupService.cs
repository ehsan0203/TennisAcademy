using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MTA.Application.Services;

public class LookupService : ILookupService
{
    private readonly IUnitOfWork _unitOfWork;

    public LookupService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<LookupDto>> GetAllAsync(int page = 1, int pageSize = 10, string? category = null, string? searchTerm = null, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var query = repo.GetQueryable()
            .AsNoTracking();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(l => l.Category == category);

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(l => l.Key.Contains(searchTerm) || l.Value.Contains(searchTerm));

        var total = await repo.CountAsync(query, ct);
        var items = await repo.GetPagedAsync(query.OrderBy(l => l.Id), page, pageSize, ct);

        return new PaginatedResult<LookupDto>
        {
            Data = items.Select(l => new LookupDto
            {
                Id = l.Id,
                Category = l.Category,
                Key = l.Key,
                Value = l.Value
            }),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<LookupDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entity = await repo.GetByIdAsync(id, ct);

        return entity == null ? null : new LookupDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Key = entity.Key,
            Value = entity.Value
        };
    }

    public async Task<IEnumerable<LookupDto>> GetByCategoryAsync(string category, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var items = await repo.GetAllAsync(l => l.Category == category, ct: ct);

        return items.Select(l => new LookupDto
        {
            Id = l.Id,
            Category = l.Category,
            Key = l.Key,
            Value = l.Value
        });
    }

    public async Task<LookupDto?> GetByCategoryAndKeyAsync(string category, string key, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entity = await repo.GetQueryable()
                               .AsNoTracking()
                               .FirstOrDefaultAsync(l => l.Category == category && l.Key == key, ct);

        return entity == null ? null : new LookupDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Key = entity.Key,
            Value = entity.Value
        };
    }

    public async Task<string?> GetValueAsync(string category, string key, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entity = await repo.GetQueryable()
                               .AsNoTracking()
                               .FirstOrDefaultAsync(l => l.Category == category && l.Key == key, ct);

        return entity?.Value;
    }

    public async Task<LookupDto> CreateAsync(CreateLookupDto lookupDto, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();

        var entity = new Lookup
        {
            Category = lookupDto.Category,
            Key = lookupDto.Key,
            Value = lookupDto.Value
        };

        await repo.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new LookupDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Key = entity.Key,
            Value = entity.Value,
        };
    }

    public async Task<LookupDto> UpdateAsync(int id, LookupDto lookupDto, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entity = await repo.GetByIdAsync(id, ct);

        if (entity == null)
            throw new KeyNotFoundException($"Lookup with ID {id} not found.");

        entity.Category = lookupDto.Category;
        entity.Key = lookupDto.Key;
        entity.Value = lookupDto.Value;

        await repo.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new LookupDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Key = entity.Key,
            Value = entity.Value
        };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var result = await repo.DeleteAsync(id, ct);
        if (result)
            await _unitOfWork.SaveChangesAsync(ct);

        return result;
    }

    public async Task<IEnumerable<string>> GetAllCategoriesAsync(CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        return await repo.GetQueryable()
                         .AsNoTracking()
                         .Select(l => l.Category)
                         .Distinct()
                         .ToListAsync(ct);
    }

    public async Task<LookupStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var query = repo.GetQueryable()
            .AsNoTracking();

        var total = await repo.CountAsync(ct: ct);
        var categories = await query.Select(l => l.Category).Distinct().ToListAsync(ct);

        var lookupsPerCategory = await query
            .GroupBy(l => l.Category)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count, ct);

        var now = DateTime.UtcNow;
        var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
        var startOfLastMonth = startOfThisMonth.AddMonths(-1);

        var lookupsThisMonth = await query.CountAsync(l => l.CreatedAt >= startOfThisMonth, ct);
        var lookupsLastMonth = await query.CountAsync(l => l.CreatedAt >= startOfLastMonth && l.CreatedAt < startOfThisMonth, ct);

        return new LookupStatisticsDto
        {
            TotalLookups = total,
            TotalCategories = categories.Count,
            LookupsPerCategory = lookupsPerCategory,
            LookupsThisMonth = lookupsThisMonth,
            LookupsLastMonth = lookupsLastMonth
        };
    }

    public async Task<IEnumerable<LookupDto>> BulkCreateAsync(IEnumerable<LookupDto> lookupDtos, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entities = lookupDtos.Select(dto => new Lookup
        {
            Category = dto.Category,
            Key = dto.Key,
            Value = dto.Value
        }).ToList();

        foreach (var entity in entities)
        {
            await repo.AddAsync(entity, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return entities.Select(e => new LookupDto
        {
            Id = e.Id,
            Category = e.Category,
            Key = e.Key,
            Value = e.Value
        });
    }


}
