using MTA.Domain.Entities;

namespace MTA.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : BaseEntity;

    IRepository<UserProfile> UserProfiles { get; }
    IRepository<Account> Accounts { get; }
    IRepository<Role> Roles { get; }
    IRepository<Level> Levels { get; }

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
