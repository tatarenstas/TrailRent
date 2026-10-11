using System.Data;
using TrailRent.Dal.Repositories;

namespace TrailRent.Dal.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    ICustomerRepository Customers { get; }
    IEquipmentRepository Equipment { get; }
    IRentalRepository Rentals { get; }
    Task BeginTransactionAsync(IsolationLevel level = IsolationLevel.ReadCommitted, CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
