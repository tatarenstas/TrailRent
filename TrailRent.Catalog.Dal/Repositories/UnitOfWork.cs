namespace TrailRent.Catalog.Dal.Repositories;

public sealed class UnitOfWork(CatalogDbContext context, IEquipmentRepository equipment, IEquipmentCategoryRepository categories, IEquipmentFeatureRepository equipmentFeatures) : IUnitOfWork
{
    public IEquipmentRepository Equipment { get; } = equipment;
    public IEquipmentCategoryRepository Categories { get; } = categories;
    public IEquipmentFeatureRepository EquipmentFeatures { get; } = equipmentFeatures;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}
