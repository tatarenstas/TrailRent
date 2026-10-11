namespace TrailRent.Catalog.Domain;

public sealed class EquipmentSpecs
{
    private EquipmentSpecs() { }
    public EquipmentSpecs(int equipmentId, decimal weightKg, string dimensions, string material)
    { EquipmentId = equipmentId; WeightKg = weightKg; Dimensions = dimensions; Material = material; }
    public int EquipmentId { get; private set; }
    public decimal WeightKg { get; private set; }
    public string Dimensions { get; private set; } = null!;
    public string Material { get; private set; } = null!;
    public Equipment Equipment { get; private set; } = null!;
}
