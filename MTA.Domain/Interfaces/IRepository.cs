using Microsoft.EntityFrameworkCore;
using MTA.Domain.Entities;
using System.Linq.Expressions;

namespace MTA.Domain.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets all entities asynchronously
    /// </summary>
    /// <returns>Collection of all entities</returns>
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Gets an entity by ID asynchronously
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(int id);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Adds a new entity asynchronously
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added entity</returns>
    Task<T> AddAsync(T entity);
    
    /// <summary>
    /// Updates an existing entity asynchronously
    /// </summary>
    /// <param name="entity">Entity to update</param>
    /// <returns>Updated entity</returns>
    Task<T> UpdateAsync(T entity);
    
    /// <summary>
    /// Deletes an entity asynchronously
    /// </summary>
    /// <param name="id">ID of entity to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Checks if an entity exists by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Returns IQueryable for LINQ queries
    /// </summary>
    IQueryable<T> GetQueryable();
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? filter = null);

    Task<int> CountAsync(IQueryable<T> query);

    Task<IEnumerable<T>> GetPagedAsync(IQueryable<T> query, int page, int pageSize);
}
