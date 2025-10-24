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

    public async Task<PaginatedResult<LookupDto>> 
        
        GetAllAsync(int page = 1, int pageSize = 10, string? category = null, string? searchTerm = null)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var query = repo.GetQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(l => l.Category == category);

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(l => l.Key.Contains(searchTerm) || l.Value.Contains(searchTerm));

        var total = await repo.CountAsync(query);
        var items = await repo.GetPagedAsync(query.OrderBy(l => l.Id), page, pageSize);

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

    public async Task<LookupDto?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entity = await repo.GetByIdAsync(id);

        return entity == null ? null : new LookupDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Key = entity.Key,
            Value = entity.Value
        };
    }

    public async Task<IEnumerable<LookupDto>> GetByCategoryAsync(string category)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var items = await repo.GetAllAsync(l => l.Category == category);

        return items.Select(l => new LookupDto
        {
            Id = l.Id,
            Category = l.Category,
            Key = l.Key,
            Value = l.Value
        });
    }

    public async Task<LookupDto?> GetByCategoryAndKeyAsync(string category, string key)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entity = await repo.GetQueryable()
                               .FirstOrDefaultAsync(l => l.Category == category && l.Key == key);

        return entity == null ? null : new LookupDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Key = entity.Key,
            Value = entity.Value
        };
    }

    public async Task<string?> GetValueAsync(string category, string key)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entity = await repo.GetQueryable()
                               .FirstOrDefaultAsync(l => l.Category == category && l.Key == key);

        return entity?.Value;
    }

    public async Task<LookupDto> CreateAsync(CreateLookupDto lookupDto)
    {
        var repo = _unitOfWork.Repository<Lookup>();

        var entity = new Lookup
        {
            Category = lookupDto.Category,
            Key = lookupDto.Key,
            Value = lookupDto.Value,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new LookupDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Key = entity.Key,
            Value = entity.Value,
        };
    }

    public async Task<LookupDto> UpdateAsync(int id, LookupDto lookupDto)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entity = await repo.GetByIdAsync(id);

        if (entity == null)
            throw new KeyNotFoundException($"Lookup with ID {id} not found.");

        entity.Category = lookupDto.Category;
        entity.Key = lookupDto.Key;
        entity.Value = lookupDto.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new LookupDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Key = entity.Key,
            Value = entity.Value
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var result = await repo.DeleteAsync(id);
        if (result)
            await _unitOfWork.SaveChangesAsync();

        return result;
    }

    public async Task<IEnumerable<string>> GetAllCategoriesAsync()
    {
        var repo = _unitOfWork.Repository<Lookup>();
        return await repo.GetQueryable()
                         .Select(l => l.Category)
                         .Distinct()
                         .ToListAsync();
    }

    public async Task<LookupStatisticsDto> GetStatisticsAsync()
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var query = repo.GetQueryable();

        var total = await repo.CountAsync();
        var categories = await query.Select(l => l.Category).Distinct().ToListAsync();

        var lookupsPerCategory = await query
            .GroupBy(l => l.Category)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count);

        var now = DateTime.UtcNow;
        var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
        var startOfLastMonth = startOfThisMonth.AddMonths(-1);

        var lookupsThisMonth = await query.CountAsync(l => l.CreatedAt >= startOfThisMonth);
        var lookupsLastMonth = await query.CountAsync(l => l.CreatedAt >= startOfLastMonth && l.CreatedAt < startOfThisMonth);

        return new LookupStatisticsDto
        {
            TotalLookups = total,
            TotalCategories = categories.Count,
            LookupsPerCategory = lookupsPerCategory,
            LookupsThisMonth = lookupsThisMonth,
            LookupsLastMonth = lookupsLastMonth
        };
    }

    public async Task<IEnumerable<LookupDto>> BulkCreateAsync(IEnumerable<LookupDto> lookupDtos)
    {
        var repo = _unitOfWork.Repository<Lookup>();
        var entities = lookupDtos.Select(dto => new Lookup
        {
            Category = dto.Category,
            Key = dto.Key,
            Value = dto.Value,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        foreach (var entity in entities)
        {
            await repo.AddAsync(entity);
        }

        await _unitOfWork.SaveChangesAsync();

        return entities.Select(e => new LookupDto
        {
            Id = e.Id,
            Category = e.Category,
            Key = e.Key,
            Value = e.Value
        });
    }


}
