using MTA.Domain.Entities;

namespace MTA.Domain.Interfaces;

/// <summary>
/// Unit of Work interface for managing transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the repository for a specific entity type
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <returns>Repository instance</returns>
    IRepository<T> Repository<T>() where T : BaseEntity;
    
    /// <summary>
    /// Begins a new transaction
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task BeginTransactionAsync();
    
    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task CommitAsync();
    
    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task RollbackAsync();
    
    /// <summary>
    /// Saves all changes made in the current context
    /// </summary>
    /// <returns>Number of affected records</returns>
    Task<int> SaveChangesAsync();
}
