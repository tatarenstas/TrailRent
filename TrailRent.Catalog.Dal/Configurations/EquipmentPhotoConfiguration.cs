using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Configurations;

public sealed class EquipmentPhotoConfiguration : IEntityTypeConfiguration<EquipmentPhoto>
{
    public void Configure(EntityTypeBuilder<EquipmentPhoto> builder)
    {
        builder.ToTable("EquipmentPhotos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
        builder.HasIndex(x => new { x.EquipmentId, x.SortOrder }).IsUnique();
        builder.HasOne(x => x.Equipment).WithMany(x => x.Photos).HasForeignKey(x => x.EquipmentId).OnDelete(DeleteBehavior.Cascade);
    }
}
