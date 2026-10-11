using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Configurations;

public sealed class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment", t => t.HasCheckConstraint("CK_Equipment_DailyRate", "[DailyRate] > 0"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Sku).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.DailyRate).HasPrecision(18, 2);
        builder.HasIndex(x => x.Sku).IsUnique();
        builder.HasOne(x => x.Category).WithMany(x => x.Equipment).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}
