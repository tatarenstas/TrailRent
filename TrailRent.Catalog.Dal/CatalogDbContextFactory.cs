using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TrailRent.Catalog.Dal;

public sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args) => new(
        new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=TrailRentCatalog;User Id=sa;Password=TrailRent!2026;TrustServerCertificate=True")
            .Options);
}
