using Microsoft.EntityFrameworkCore;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using MTA.Infrastructure.Data;
using System.Linq.Expressions;

namespace MTA.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> GetQueryable() => _dbSet.AsQueryable();

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.AsNoTracking().ToListAsync(ct);

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

    public async Task<IEnumerable<T>> GetAllAsync(
        Func<IQueryable<T>, IQueryable<T>>? include = null, CancellationToken ct = default)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        if (include != null) query = include(query);
        return await query.ToListAsync(ct);
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _dbSet.Where(e => e.Id == id).FirstOrDefaultAsync(ct);

    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.AnyAsync(predicate, ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        return entity;
    }

    public Task<T> UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        return Task.FromResult(entity);
    }

    // Soft delete — sets IsDeleted = true instead of physically removing
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null) return false;

        entity.IsDeleted = true;
        _dbSet.Update(entity);
        return true;
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await _dbSet.AnyAsync(e => e.Id == id, ct);

    public async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        => predicate != null
            ? await _dbSet.CountAsync(predicate, ct)
            : await _dbSet.CountAsync(ct);

    public async Task<int> CountAsync(IQueryable<T> query, CancellationToken ct = default)
        => await query.CountAsync(ct);

    public async Task<IEnumerable<T>> GetPagedAsync(
        int page, int pageSize,
        Expression<Func<T, bool>>? filter = null,
        CancellationToken ct = default)
    {
        var query = filter != null ? _dbSet.AsNoTracking().Where(filter) : _dbSet.AsNoTracking().AsQueryable();
        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<IEnumerable<T>> GetPagedAsync(
        IQueryable<T> query, int page, int pageSize, CancellationToken ct = default)
        => await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public async Task<IEnumerable<T>> WhereAsync(
        Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(predicate, ct);

    public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        foreach (var entity in entities)
            entity.IsDeleted = true;
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }
}
