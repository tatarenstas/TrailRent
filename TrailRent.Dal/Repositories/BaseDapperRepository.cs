using Dapper;
using TrailRent.Domain.Entities;

namespace TrailRent.Dal.Repositories;

public abstract class BaseDapperRepository<T>(Microsoft.Data.SqlClient.SqlConnection connection,
    Func<Microsoft.Data.SqlClient.SqlTransaction?> currentTransaction, string tableName)
    : RepositoryBase(connection, currentTransaction), IGenericRepository<T> where T : class, IEntity
{
    private readonly string _tableName = tableName; // Константа коду, не вводиться користувачем.

    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        await EnsureOpenAsync(ct);
        return await Connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            $"SELECT * FROM {_tableName} WHERE Id = @Id AND IsDeleted = 0", new { Id = id },
            Transaction, cancellationToken: ct));
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        await EnsureOpenAsync(ct);
        var result = await Connection.QueryAsync<T>(new CommandDefinition(
            $"SELECT * FROM {_tableName} WHERE IsDeleted = 0 ORDER BY Id", transaction: Transaction,
            cancellationToken: ct));
        return result.AsList();
    }

    public abstract Task<long> AddAsync(T entity, CancellationToken ct = default);
    public abstract Task<bool> UpdateAsync(T entity, CancellationToken ct = default);

    public virtual async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        await EnsureOpenAsync(ct);
        var changed = await Connection.ExecuteAsync(new CommandDefinition(
            $"UPDATE {_tableName} SET IsDeleted = 1, UpdatedAt = SYSUTCDATETIME() WHERE Id = @Id AND IsDeleted = 0",
            new { Id = id }, Transaction, cancellationToken: ct));
        return changed == 1;
    }
}
