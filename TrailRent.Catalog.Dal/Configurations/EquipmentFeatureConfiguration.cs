using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Configurations;

public sealed class EquipmentFeatureConfiguration : IEntityTypeConfiguration<EquipmentFeature>
{
    public void Configure(EntityTypeBuilder<EquipmentFeature> builder)
    {
        builder.ToTable("EquipmentFeatures");
        builder.HasKey(x => new { x.EquipmentId, x.FeatureId });
        builder.HasOne(x => x.Equipment).WithMany(x => x.EquipmentFeatures).HasForeignKey(x => x.EquipmentId);
        builder.HasOne(x => x.Feature).WithMany(x => x.EquipmentFeatures).HasForeignKey(x => x.FeatureId);
    }
}
