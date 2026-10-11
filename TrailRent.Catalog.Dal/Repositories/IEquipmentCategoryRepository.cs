using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Repositories;

public interface IEquipmentCategoryRepository : IRepository<EquipmentCategory>
{
    Task<EquipmentCategory?> GetWithEquipmentAsync(int id, CancellationToken cancellationToken = default);
}
