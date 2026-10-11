using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Configurations;

public sealed class EquipmentSpecsConfiguration : IEntityTypeConfiguration<EquipmentSpecs>
{
    public void Configure(EntityTypeBuilder<EquipmentSpecs> builder)
    {
        builder.ToTable("EquipmentSpecs", t => t.HasCheckConstraint("CK_EquipmentSpecs_Weight", "[WeightKg] > 0"));
        builder.HasKey(x => x.EquipmentId);
        builder.Property(x => x.WeightKg).HasPrecision(18, 2);
        builder.Property(x => x.Dimensions).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Material).HasMaxLength(80).IsRequired();
        builder.HasOne(x => x.Equipment).WithOne(x => x.Specs).HasForeignKey<EquipmentSpecs>(x => x.EquipmentId).OnDelete(DeleteBehavior.Cascade);
    }
}
