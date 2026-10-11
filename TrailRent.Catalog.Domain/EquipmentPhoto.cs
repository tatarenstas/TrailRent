namespace TrailRent.Catalog.Domain;

public sealed class EquipmentPhoto
{
    private EquipmentPhoto() { }
    public EquipmentPhoto(int equipmentId, string url, int sortOrder) { EquipmentId = equipmentId; Url = url; SortOrder = sortOrder; }
    public int Id { get; private set; }
    public int EquipmentId { get; private set; }
    public string Url { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public Equipment Equipment { get; private set; } = null!;
}
