using Dapper;
using Microsoft.Data.SqlClient;
using TrailRent.Domain.Entities;
using TrailRent.Domain.Exceptions;

namespace TrailRent.Dal.Repositories;

public sealed class EquipmentRepository(SqlConnection connection, Func<SqlTransaction?> currentTransaction)
    : BaseDapperRepository<Equipment>(connection, currentTransaction, "dbo.Equipment"), IEquipmentRepository
{
    public override async Task<long> AddAsync(Equipment item, CancellationToken ct = default)
    {
        const string sql = """
            INSERT dbo.Equipment (Name, DailyRate, AvailableUnits, IsRentable)
            OUTPUT INSERTED.Id VALUES (@Name, @DailyRate, @AvailableUnits, @IsRentable);
            """;
        await EnsureOpenAsync(ct);
        return await Connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, item, Transaction, cancellationToken: ct));
    }

    public override async Task<bool> UpdateAsync(Equipment item, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE dbo.Equipment SET Name = @Name, DailyRate = @DailyRate, AvailableUnits = @AvailableUnits,
                IsRentable = @IsRentable, UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @Id AND IsDeleted = 0;
            """;
        await EnsureOpenAsync(ct);
        return await Connection.ExecuteAsync(new CommandDefinition(sql, item, Transaction, cancellationToken: ct)) == 1;
    }

    public async Task<Equipment?> GetRentableByIdAsync(long id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM dbo.Equipment WHERE Id = @Id AND IsDeleted = 0 AND IsRentable = 1;";
        await EnsureOpenAsync(ct);
        return await Connection.QuerySingleOrDefaultAsync<Equipment>(new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct));
    }

    public async Task DecreaseAvailabilityAsync(long equipmentId, int quantity, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE dbo.Equipment SET AvailableUnits = AvailableUnits - @Quantity, UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @EquipmentId AND IsDeleted = 0 AND IsRentable = 1 AND AvailableUnits >= @Quantity;
            """;
        await EnsureOpenAsync(ct);
        var updated = await Connection.ExecuteAsync(new CommandDefinition(sql, new { EquipmentId = equipmentId, Quantity = quantity }, Transaction, cancellationToken: ct));
        if (updated != 1) throw new BusinessConflictException($"Equipment {equipmentId} is unavailable in the requested quantity.");
    }
}
