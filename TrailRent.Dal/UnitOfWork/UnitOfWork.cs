using System.Data;
using Microsoft.Data.SqlClient;
using TrailRent.Dal.Repositories;

namespace TrailRent.Dal.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SqlConnection _connection;
    private SqlTransaction? _transaction;

    public UnitOfWork(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
        Customers = new CustomerRepository(_connection, () => _transaction);
        Equipment = new EquipmentRepository(_connection, () => _transaction);
        Rentals = new RentalRepository(_connection, () => _transaction);
    }

    public ICustomerRepository Customers { get; }
    public IEquipmentRepository Equipment { get; }
    public IRentalRepository Rentals { get; }

    public async Task BeginTransactionAsync(IsolationLevel level = IsolationLevel.ReadCommitted, CancellationToken ct = default)
    {
        if (_transaction is not null) throw new InvalidOperationException("Transaction is already active.");
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync(ct);
        _transaction = (SqlTransaction)await _connection.BeginTransactionAsync(level, ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction is null) throw new InvalidOperationException("Transaction has not started.");
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

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null) await _transaction.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
