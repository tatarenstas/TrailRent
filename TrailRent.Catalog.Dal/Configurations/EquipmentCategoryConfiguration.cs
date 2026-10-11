using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Configurations;

public sealed class EquipmentCategoryConfiguration : IEntityTypeConfiguration<EquipmentCategory>
{
    public void Configure(EntityTypeBuilder<EquipmentCategory> builder)
    {
        builder.ToTable("EquipmentCategories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasData(new EquipmentCategory(1, "Camping", "Tents and camp furniture"), new EquipmentCategory(2, "Hiking", "Trail equipment"));
    }
}
