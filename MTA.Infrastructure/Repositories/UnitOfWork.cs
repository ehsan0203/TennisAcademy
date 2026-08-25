using Microsoft.EntityFrameworkCore.Storage;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using MTA.Infrastructure.Data;

namespace MTA.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            var repoType = typeof(Repository<>).MakeGenericType(type);
            _repositories[type] = Activator.CreateInstance(repoType, _context)!;
        }
        return (IRepository<T>)_repositories[type];
    }

    public IRepository<UserProfile> UserProfiles => Repository<UserProfile>();
    public IRepository<Account> Accounts => Repository<Account>();
    public IRepository<Role> Roles => Repository<Role>();
    public IRepository<Level> Levels => Repository<Level>();

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await _context.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction is null) return;
        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
