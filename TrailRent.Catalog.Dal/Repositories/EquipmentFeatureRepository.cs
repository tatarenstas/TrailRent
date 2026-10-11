using Microsoft.EntityFrameworkCore;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Repositories;

public sealed class EquipmentFeatureRepository(CatalogDbContext context) : EfRepository<EquipmentFeature>(context), IEquipmentFeatureRepository
{
    // M:N is queried through the explicit EquipmentFeature join entity.
    public async Task<IReadOnlyList<Equipment>> GetEquipmentByFeatureSinceAsync(string featureName, DateTime since, CancellationToken cancellationToken = default) =>
        await Context.Equipment.AsNoTracking().Where(e => e.EquipmentFeatures.Any(link => link.Feature.Name == featureName && link.AddedAt >= since))
            .Include(e => e.Category).OrderBy(e => e.Name).ToListAsync(cancellationToken);
}
