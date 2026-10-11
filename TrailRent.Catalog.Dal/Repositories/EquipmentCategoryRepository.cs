using Microsoft.EntityFrameworkCore;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Repositories;

public sealed class EquipmentCategoryRepository(CatalogDbContext context) : EfRepository<EquipmentCategory>(context), IEquipmentCategoryRepository
{
    // Explicit loading is deliberate here: category is loaded, then its collection on demand.
    public async Task<EquipmentCategory?> GetWithEquipmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await Context.EquipmentCategories.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is not null)
            await Context.Entry(category).Collection(c => c.Equipment).Query().OrderBy(e => e.Name).LoadAsync(cancellationToken);
        return category;
    }
}
