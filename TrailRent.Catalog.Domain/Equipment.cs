namespace TrailRent.Catalog.Domain;

public sealed class Equipment
{
    private Equipment() { }
    public Equipment(string sku, string name, decimal dailyRate, int availableUnits, int categoryId)
    {
        if (dailyRate <= 0) throw new ValidationException("Daily rate must be positive.");
        if (availableUnits < 0) throw new ValidationException("Available units cannot be negative.");
        Sku = sku; Name = name; DailyRate = dailyRate; AvailableUnits = availableUnits; CategoryId = categoryId;
    }

    public int Id { get; private set; }
    public string Sku { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public decimal DailyRate { get; private set; }
    public int AvailableUnits { get; private set; }
    public bool IsRentable { get; private set; } = true;
    public int CategoryId { get; private set; }
    public EquipmentCategory Category { get; private set; } = null!;
    public EquipmentSpecs? Specs { get; private set; }
    public MaintenancePolicy? MaintenancePolicy { get; private set; }
    public ICollection<EquipmentPhoto> Photos { get; } = new List<EquipmentPhoto>();
    public ICollection<EquipmentFeature> EquipmentFeatures { get; } = new List<EquipmentFeature>();

    public void ChangeDailyRate(decimal rate)
    {
        if (rate <= 0) throw new ValidationException("Daily rate must be positive.");
        DailyRate = rate;
    }
    public void RemoveFromStock(int units)
    {
        if (units <= 0 || units > AvailableUnits) throw new ConflictException("There are not enough units in stock.");
        AvailableUnits -= units;
    }
    public void AddStock(int units)
    {
        if (units <= 0) throw new ValidationException("Units must be positive.");
        AvailableUnits += units;
    }
}
