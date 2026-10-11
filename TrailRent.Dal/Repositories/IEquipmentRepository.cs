using TrailRent.Domain.Entities;

namespace TrailRent.Dal.Repositories;

public interface IEquipmentRepository : IGenericRepository<Equipment>
{
    Task<Equipment?> GetRentableByIdAsync(long id, CancellationToken ct = default);
    Task DecreaseAvailabilityAsync(long equipmentId, int quantity, CancellationToken ct = default);
}
