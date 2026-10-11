using Dapper;
using Microsoft.Data.SqlClient;

namespace TrailRent.Api.Infrastructure;

public sealed class DatabaseInitializer(string connectionString, string contentRootPath)
{
    public async Task InitializeAsync(CancellationToken ct)
    {
        var scriptPath = Path.Combine(contentRootPath, "Database", "initialize.sql");
        var script = await File.ReadAllTextAsync(scriptPath, ct);
        await using var connection = new SqlConnection(connectionString);
        foreach (var batch in script.Split("\nGO", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            await connection.ExecuteAsync(new CommandDefinition(batch, cancellationToken: ct));
        }
    }
}
