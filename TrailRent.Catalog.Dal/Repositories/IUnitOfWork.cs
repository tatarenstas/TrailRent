using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Repositories;

public interface IUnitOfWork
{
    IEquipmentRepository Equipment { get; }
    IEquipmentCategoryRepository Categories { get; }
    IEquipmentFeatureRepository EquipmentFeatures { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
