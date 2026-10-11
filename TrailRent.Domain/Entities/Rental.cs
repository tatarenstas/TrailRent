namespace TrailRent.Domain.Entities;

public sealed class Rental : IEntity
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Status { get; set; } = RentalStatuses.Draft;
    public decimal TotalAmount { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public List<RentalItem> Items { get; set; } = [];
}

public static class RentalStatuses
{
    public const string Draft = "Draft";
    public const string AwaitingPickup = "AwaitingPickup";
    public const string Active = "Active";
    public const string Returned = "Returned";
    public const string Cancelled = "Cancelled";
}
