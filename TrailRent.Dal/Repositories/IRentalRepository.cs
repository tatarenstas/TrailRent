using TrailRent.Domain.Entities;

namespace TrailRent.Dal.Repositories;

public interface IRentalRepository : IGenericRepository<Rental>
{
    Task<Rental?> GetWithItemsAsync(long id, CancellationToken ct = default);
    Task AddItemsAsync(long rentalId, IReadOnlyCollection<RentalItem> items, CancellationToken ct = default);
    Task ConfirmAsync(long rentalId, CancellationToken ct = default);
}
