namespace TrailRent.Catalog.Domain;

public sealed class Feature
{
    private Feature() { }
    public Feature(int id, string name) { Id = id; Name = name; }
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public ICollection<EquipmentFeature> EquipmentFeatures { get; } = new List<EquipmentFeature>();
}
