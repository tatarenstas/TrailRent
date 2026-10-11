using System.Data;
using Microsoft.Data.SqlClient;

namespace TrailRent.Dal.Repositories;

public abstract class RepositoryBase(SqlConnection connection, Func<SqlTransaction?> currentTransaction)
{
    protected SqlConnection Connection { get; } = connection;
    protected SqlTransaction? Transaction => currentTransaction();

    protected async Task EnsureOpenAsync(CancellationToken ct)
    {
        if (Connection.State != ConnectionState.Open)
        {
            await Connection.OpenAsync(ct);
        }
    }
}
