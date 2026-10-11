namespace TrailRent.Domain.Entities;

public sealed class Equipment : IEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public int AvailableUnits { get; set; }
    public bool IsRentable { get; set; } = true;
}
