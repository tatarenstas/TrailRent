namespace TrailRent.Catalog.Domain;

public sealed class EquipmentCategory
{
    private EquipmentCategory() { }
    public EquipmentCategory(int id, string name, string description)
    {
        Id = id; Name = name; Description = description;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ICollection<Equipment> Equipment { get; } = new List<Equipment>();
}
