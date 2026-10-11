using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Configurations;

public sealed class MaintenancePolicyConfiguration : IEntityTypeConfiguration<MaintenancePolicy>
{
    public void Configure(EntityTypeBuilder<MaintenancePolicy> builder)
    {
        builder.ToTable("MaintenancePolicies", t => t.HasCheckConstraint("CK_Maintenance_Interval", "[ServiceIntervalDays] > 0"));
        builder.HasKey(x => x.EquipmentId);
        builder.Property(x => x.Instructions).HasMaxLength(600).IsRequired();
        builder.HasOne(x => x.Equipment).WithOne(x => x.MaintenancePolicy).HasForeignKey<MaintenancePolicy>(x => x.EquipmentId).OnDelete(DeleteBehavior.Cascade);
    }
}
