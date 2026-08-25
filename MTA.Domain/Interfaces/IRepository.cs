using Microsoft.EntityFrameworkCore;
using MTA.Domain.Entities;
using System.Linq.Expressions;

namespace MTA.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null, CancellationToken ct = default);

    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task<T> UpdateAsync(T entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    IQueryable<T> GetQueryable();
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
    Task<int> CountAsync(IQueryable<T> query, CancellationToken ct = default);

    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);
    Task<IEnumerable<T>> GetPagedAsync(IQueryable<T> query, int page, int pageSize, CancellationToken ct = default);

    Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
}
