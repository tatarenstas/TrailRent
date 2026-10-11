using System.Data;
using Microsoft.Data.SqlClient;
using TrailRent.Domain.Entities;

namespace TrailRent.Dal.Repositories;

public sealed class CustomerRepository(SqlConnection connection, Func<SqlTransaction?> currentTransaction)
    : RepositoryBase(connection, currentTransaction), ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT Id, FirstName, LastName, Email, PhoneNumber, IsActive
            FROM dbo.Customers WHERE Id = @Id AND IsDeleted = 0;
            """;
        await EnsureOpenAsync(ct);
        await using var command = new SqlCommand(sql, Connection, Transaction);
        command.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;
        await using var reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        return Map(reader);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT Id, FirstName, LastName, Email, PhoneNumber, IsActive FROM dbo.Customers WHERE IsDeleted = 0 ORDER BY Id;";
        await EnsureOpenAsync(ct);
        await using var command = new SqlCommand(sql, Connection, Transaction);
        await using var reader = await command.ExecuteReaderAsync(ct);
        var customers = new List<Customer>();
        while (await reader.ReadAsync(ct)) customers.Add(Map(reader));
        return customers;
    }

    public async Task<long> AddAsync(Customer customer, CancellationToken ct = default)
    {
        const string sql = """
            INSERT dbo.Customers (FirstName, LastName, Email, PhoneNumber)
            OUTPUT INSERTED.Id VALUES (@FirstName, @LastName, @Email, @PhoneNumber);
            """;
        await EnsureOpenAsync(ct);
        await using var command = new SqlCommand(sql, Connection, Transaction);
        command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = customer.FirstName;
        command.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = customer.LastName;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = customer.Email;
        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = (object?)customer.PhoneNumber ?? DBNull.Value;
        return Convert.ToInt64(await command.ExecuteScalarAsync(ct));
    }

    public async Task<bool> UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE dbo.Customers SET FirstName = @FirstName, LastName = @LastName, Email = @Email,
                PhoneNumber = @PhoneNumber, IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @Id AND IsDeleted = 0;
            """;
        await EnsureOpenAsync(ct);
        await using var command = new SqlCommand(sql, Connection, Transaction);
        command.Parameters.Add("@Id", SqlDbType.BigInt).Value = customer.Id;
        command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = customer.FirstName;
        command.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = customer.LastName;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = customer.Email;
        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = (object?)customer.PhoneNumber ?? DBNull.Value;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = customer.IsActive;
        return await command.ExecuteNonQueryAsync(ct) == 1;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        const string sql = "UPDATE dbo.Customers SET IsDeleted = 1, UpdatedAt = SYSUTCDATETIME() WHERE Id = @Id AND IsDeleted = 0;";
        await EnsureOpenAsync(ct);
        await using var command = new SqlCommand(sql, Connection, Transaction);
        command.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;
        return await command.ExecuteNonQueryAsync(ct) == 1;
    }

    private static Customer Map(SqlDataReader reader) => new()
    {
        Id = reader.GetInt64(0), FirstName = reader.GetString(1), LastName = reader.GetString(2),
        Email = reader.GetString(3), PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
        IsActive = reader.GetBoolean(5)
    };
}
