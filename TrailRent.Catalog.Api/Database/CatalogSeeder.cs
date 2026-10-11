using Microsoft.EntityFrameworkCore;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal;

public static class CatalogSeeder
{
    // Code seed is idempotent and is called only outside Production.
    public static async Task SeedAsync(CatalogDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Equipment.AnyAsync(cancellationToken)) return;
        var tent = new Equipment("TENT-ALP-2", "Alpine two-person tent", 420m, 5, 1);
        var poles = new Equipment("POLE-CARB-01", "Carbon trekking poles", 175m, 12, 2);
        await context.Equipment.AddRangeAsync([tent, poles], cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await context.EquipmentSpecs.AddRangeAsync([new EquipmentSpecs(tent.Id, 2.45m, "220x130x105 cm", "Ripstop nylon"), new EquipmentSpecs(poles.Id, 0.42m, "65-135 cm", "Carbon fiber")], cancellationToken);
        await context.MaintenancePolicies.AddRangeAsync([new MaintenancePolicy(tent.Id, 90, "Dry fully before packing."), new MaintenancePolicy(poles.Id, 180, "Inspect locking mechanisms.")], cancellationToken);
        await context.EquipmentPhotos.AddAsync(new EquipmentPhoto(tent.Id, "https://images.example.test/tent.jpg", 1), cancellationToken);
        var waterproof = await context.Features.FindAsync([1], cancellationToken);
        var ultralight = await context.Features.FindAsync([2], cancellationToken);
        context.EquipmentFeatures.AddRange(new EquipmentFeature(tent.Id, waterproof!.Id, DateTime.UtcNow), new EquipmentFeature(poles.Id, ultralight!.Id, DateTime.UtcNow));
        await context.SaveChangesAsync(cancellationToken);
    }
}
