using Microsoft.EntityFrameworkCore;

namespace TrailRent.Orders;

// Persistent idempotency table: EventId has a database UNIQUE index.
public sealed class ProcessedIntegrationEvent { public Guid EventId { get; set; } public DateTimeOffset ProcessedAt { get; set; } }
public sealed class LocalEquipmentReference { public Guid EquipmentId { get; set; } public string Name { get; set; } = string.Empty; }
public sealed class OrdersEventDbContext(DbContextOptions<OrdersEventDbContext> options) : DbContext(options)
{
    public DbSet<ProcessedIntegrationEvent> ProcessedIntegrationEvents => Set<ProcessedIntegrationEvent>();
    public DbSet<LocalEquipmentReference> EquipmentReferences => Set<LocalEquipmentReference>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessedIntegrationEvent>().HasKey(x => x.EventId);
        modelBuilder.Entity<ProcessedIntegrationEvent>().HasIndex(x => x.EventId).IsUnique();
        modelBuilder.Entity<LocalEquipmentReference>().HasKey(x => x.EquipmentId);
    }
}
