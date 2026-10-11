using Microsoft.EntityFrameworkCore;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Repositories;

public sealed class EquipmentRepository(CatalogDbContext context) : EfRepository<Equipment>(context), IEquipmentRepository
{
    // Eager loading: all navigation data is emitted as one Include/ThenInclude query.
    public Task<Equipment?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        Context.Equipment.Include(e => e.Category).Include(e => e.Specs).Include(e => e.MaintenancePolicy)
            .Include(e => e.Photos).Include(e => e.EquipmentFeatures).ThenInclude(ef => ef.Feature)
            .AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
}
