using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Repositories;

public interface IEquipmentRepository : IRepository<Equipment>
{
    Task<Equipment?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
