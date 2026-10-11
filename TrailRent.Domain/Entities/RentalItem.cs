namespace TrailRent.Domain.Entities;

public sealed class RentalItem
{
    public long RentalId { get; set; }
    public long EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public int Quantity { get; set; }
}
