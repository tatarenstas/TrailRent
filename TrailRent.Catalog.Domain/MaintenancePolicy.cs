namespace TrailRent.Catalog.Domain;

public sealed class MaintenancePolicy
{
    private MaintenancePolicy() { }
    public MaintenancePolicy(int equipmentId, int serviceIntervalDays, string instructions)
    { EquipmentId = equipmentId; ServiceIntervalDays = serviceIntervalDays; Instructions = instructions; }
    public int EquipmentId { get; private set; }
    public int ServiceIntervalDays { get; private set; }
    public string Instructions { get; private set; } = null!;
    public Equipment Equipment { get; private set; } = null!;
}
