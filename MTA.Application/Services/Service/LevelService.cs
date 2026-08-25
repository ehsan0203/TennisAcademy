using Microsoft.EntityFrameworkCore;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services.Service;

public class LevelService : ILevelService
{
    private readonly IUnitOfWork _unitOfWork;

    public LevelService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<LevelDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Level>().GetQueryable().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(l => l.Title.Contains(searchTerm));

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(l => l.Title)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<LevelDto>
        {
            Data = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<LevelDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var level = await _unitOfWork.Repository<Level>().GetByIdAsync(id, ct);
        return level is null ? null : MapToDto(level);
    }

    public async Task<LevelDto?> GetByTitleAsync(string title, CancellationToken ct = default)
    {
        var level = await _unitOfWork.Repository<Level>().GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Title.ToLower() == title.ToLower(), ct);
        return level is null ? null : MapToDto(level);
    }

    public async Task<LevelDto> CreateAsync(LevelDto levelDto, CancellationToken ct = default)
    {
        var level = new Level { Title = levelDto.Title };
        await _unitOfWork.Repository<Level>().AddAsync(level, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(level);
    }

    public async Task<LevelDto> UpdateAsync(int id, LevelDto levelDto, CancellationToken ct = default)
    {
        var level = await _unitOfWork.Repository<Level>().GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Level with ID {id} not found");

        level.Title = levelDto.Title;
        await _unitOfWork.Repository<Level>().UpdateAsync(level, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(level);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var deleted = await _unitOfWork.Repository<Level>().DeleteAsync(id, ct);
        if (deleted) await _unitOfWork.SaveChangesAsync(ct);
        return deleted;
    }

    public async Task<LevelStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        var levels = await _unitOfWork.Repository<Level>().GetQueryable()
            .AsNoTracking()
            .Include(l => l.Courses)
            .Include(l => l.Profiles)
            .ToListAsync(ct);

        return new LevelStatisticsDto
        {
            TotalLevels = levels.Count,
            LevelsWithCourses = levels.Count(l => l.Courses.Any()),
            LevelsWithoutCourses = levels.Count(l => !l.Courses.Any()),
            LevelsWithUsers = levels.Count(l => l.Profiles.Any()),
            LevelsWithoutUsers = levels.Count(l => !l.Profiles.Any()),
            AverageCoursesPerLevel = levels.Any() ? levels.Average(l => l.Courses.Count) : 0,
            AverageUsersPerLevel = levels.Any() ? levels.Average(l => l.Profiles.Count) : 0
        };
    }

    public async Task<IEnumerable<LevelDto>> GetLevelsWithCourseCountAsync(CancellationToken ct = default)
    {
        var levels = await _unitOfWork.Repository<Level>().GetAllAsync(
            include: q => q.Include(l => l.Courses), ct);
        return levels.Select(MapToDto);
    }

    public async Task<IEnumerable<LevelDto>> GetLevelsWithUserCountAsync(CancellationToken ct = default)
    {
        var levels = await _unitOfWork.Repository<Level>().GetAllAsync(
            include: q => q.Include(l => l.Profiles), ct);
        return levels.Select(MapToDto);
    }

    public async Task<IEnumerable<LevelDto>> GetLevelsOrderedByDifficultyAsync(CancellationToken ct = default)
    {
        var levels = await _unitOfWork.Repository<Level>().GetAllAsync(ct);
        return levels.OrderBy(l => l.Id).Select(MapToDto);
    }

    public async Task<IEnumerable<LevelDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var levels = await _unitOfWork.Repository<Level>()
            .GetAllAsync(l => l.CreatedAt >= startDate && l.CreatedAt <= endDate, ct);
        return levels.Select(MapToDto);
    }

    private static LevelDto MapToDto(Level l) => new()
    {
        Id = l.Id,
        Title = l.Title,
        CourseCount = l.Courses?.Count ?? 0,
        UserCount = l.Profiles?.Count ?? 0
    };
}
