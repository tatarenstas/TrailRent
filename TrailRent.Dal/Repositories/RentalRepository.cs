using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TrailRent.Domain.Entities;
using TrailRent.Domain.Exceptions;

namespace TrailRent.Dal.Repositories;

public sealed class RentalRepository(SqlConnection connection, Func<SqlTransaction?> currentTransaction)
    : BaseDapperRepository<Rental>(connection, currentTransaction, "dbo.Rentals"), IRentalRepository
{
    public override async Task<long> AddAsync(Rental rental, CancellationToken ct = default)
    {
        const string sql = """
            INSERT dbo.Rentals (CustomerId, StartDate, EndDate, Status, TotalAmount, PickupLocation)
            OUTPUT INSERTED.Id VALUES (@CustomerId, @StartDate, @EndDate, @Status, @TotalAmount, @PickupLocation);
            """;
        await EnsureOpenAsync(ct);
        return await Connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, rental, Transaction, cancellationToken: ct));
    }

    public override async Task<bool> UpdateAsync(Rental rental, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE dbo.Rentals SET StartDate = @StartDate, EndDate = @EndDate, PickupLocation = @PickupLocation,
                UpdatedAt = SYSUTCDATETIME() WHERE Id = @Id AND IsDeleted = 0 AND Status = 'Draft';
            """;
        await EnsureOpenAsync(ct);
        return await Connection.ExecuteAsync(new CommandDefinition(sql, rental, Transaction, cancellationToken: ct)) == 1;
    }

    public async Task AddItemsAsync(long rentalId, IReadOnlyCollection<RentalItem> items, CancellationToken ct = default)
    {
        const string sql = """
            INSERT dbo.RentalItems (RentalId, EquipmentId, EquipmentName, DailyRate, Quantity)
            VALUES (@RentalId, @EquipmentId, @EquipmentName, @DailyRate, @Quantity);
            """;
        await EnsureOpenAsync(ct);
        foreach (var item in items)
        {
            item.RentalId = rentalId;
            await Connection.ExecuteAsync(new CommandDefinition(sql, item, Transaction, cancellationToken: ct));
        }
    }

    public async Task<Rental?> GetWithItemsAsync(long id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT r.Id, r.CustomerId, r.StartDate, r.EndDate, r.Status, r.TotalAmount, r.PickupLocation,
                   i.RentalId, i.EquipmentId, i.EquipmentName, i.DailyRate, i.Quantity
            FROM dbo.Rentals r LEFT JOIN dbo.RentalItems i ON i.RentalId = r.Id
            WHERE r.Id = @Id AND r.IsDeleted = 0;
            """;
        await EnsureOpenAsync(ct);
        Rental? result = null;
        await Connection.QueryAsync<Rental, RentalItem?, Rental>(new CommandDefinition(sql, new { Id = id }, Transaction, cancellationToken: ct),
            (rental, item) => { result ??= rental; if (item?.EquipmentId > 0) result.Items.Add(item); return result; },
            splitOn: "RentalId");
        return result;
    }

    public async Task ConfirmAsync(long rentalId, CancellationToken ct = default)
    {
        try
        {
            await EnsureOpenAsync(ct);
            await Connection.ExecuteAsync(new CommandDefinition("dbo.usp_ConfirmRental", new { RentalId = rentalId }, Transaction,
                commandType: CommandType.StoredProcedure, cancellationToken: ct));
        }
        catch (SqlException ex) when (ex.Number == 51001)
        {
            throw new NotFoundException($"Rental {rentalId} was not found.");
        }
        catch (SqlException ex) when (ex.Number == 51002)
        {
            throw new BusinessConflictException("Only a draft rental can be confirmed.");
        }
    }
}
