using Microsoft.EntityFrameworkCore;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<EquipmentCategory> EquipmentCategories => Set<EquipmentCategory>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<EquipmentSpecs> EquipmentSpecs => Set<EquipmentSpecs>();
    public DbSet<MaintenancePolicy> MaintenancePolicies => Set<MaintenancePolicy>();
    public DbSet<EquipmentPhoto> EquipmentPhotos => Set<EquipmentPhoto>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<EquipmentFeature> EquipmentFeatures => Set<EquipmentFeature>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
