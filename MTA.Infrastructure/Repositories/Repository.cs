using Microsoft.EntityFrameworkCore;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using MTA.Infrastructure.Data;
using System.Linq.Expressions;

namespace MTA.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AnyAsync(predicate);
    }

    /// <summary>
    /// Returns IQueryable for LINQ queries
    /// </summary>
    public IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    /// <summary>
    /// Gets all entities asynchronously
    /// </summary>
    /// <returns>Collection of all entities</returns>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate) =>
        predicate != null ? await _dbSet.Where(predicate).ToListAsync() : await GetAllAsync();


    /// <summary>
    /// Gets an entity by ID asynchronously
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Entity if found, null otherwise</returns>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Adds a new entity asynchronously
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added entity</returns>
    public virtual async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Updates an existing entity asynchronously
    /// </summary>
    /// <param name="entity">Entity to update</param>
    /// <returns>Updated entity</returns>
    public virtual async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        return entity;
    }

    /// <summary>
    /// Deletes an entity asynchronously
    /// </summary>
    /// <param name="id">ID of entity to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        return true;
    }

    /// <summary>
    /// Checks if an entity exists by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>True if exists, false otherwise</returns>
    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) =>
    predicate != null ? await _dbSet.CountAsync(predicate) : await _dbSet.CountAsync();

    public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? filter = null)
    {
        var query = filter != null ? _dbSet.Where(filter) : _dbSet;
        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<int> CountAsync(IQueryable<T> query) => await query.CountAsync();

    public async Task<IEnumerable<T>> GetPagedAsync(IQueryable<T> query, int page, int pageSize) =>
        await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

}
