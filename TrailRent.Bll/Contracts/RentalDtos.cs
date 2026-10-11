using System.ComponentModel.DataAnnotations;

namespace TrailRent.Bll.Contracts;

public sealed record RentalItemDto(long EquipmentId, string EquipmentName, decimal DailyRate, int Quantity);
public sealed record RentalDto(long Id, long CustomerId, DateOnly StartDate, DateOnly EndDate, string Status,
    decimal TotalAmount, string PickupLocation, IReadOnlyList<RentalItemDto> Items);

public sealed class CreateRentalRequest
{
    [Range(1, long.MaxValue)] public long CustomerId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    [Required, StringLength(150)] public string PickupLocation { get; init; } = string.Empty;
    [Required, MinLength(1)] public List<CreateRentalItemRequest> Items { get; init; } = [];
}

public sealed class CreateRentalItemRequest
{
    [Range(1, long.MaxValue)] public long EquipmentId { get; init; }
    [Range(1, 20)] public int Quantity { get; init; }
}
