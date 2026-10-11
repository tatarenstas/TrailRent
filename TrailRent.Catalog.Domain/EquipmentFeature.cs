namespace TrailRent.Catalog.Domain;

public sealed class EquipmentFeature
{
    private EquipmentFeature() { }
    public EquipmentFeature(int equipmentId, int featureId, DateTime addedAt)
    { EquipmentId = equipmentId; FeatureId = featureId; AddedAt = addedAt; }
    public int EquipmentId { get; private set; }
    public int FeatureId { get; private set; }
    public DateTime AddedAt { get; private set; }
    public Equipment Equipment { get; private set; } = null!;
    public Feature Feature { get; private set; } = null!;
}
