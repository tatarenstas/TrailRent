using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Repositories;

public interface IEquipmentFeatureRepository : IRepository<EquipmentFeature>
{
    Task<IReadOnlyList<Equipment>> GetEquipmentByFeatureSinceAsync(string featureName, DateTime since, CancellationToken cancellationToken = default);
}
