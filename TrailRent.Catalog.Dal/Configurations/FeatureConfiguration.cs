using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Configurations;

public sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("Features");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(80).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasData(new Feature(1, "Waterproof"), new Feature(2, "Ultralight"), new Feature(3, "Four-season"));
    }
}
