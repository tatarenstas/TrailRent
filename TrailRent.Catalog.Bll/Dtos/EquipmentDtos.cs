using System.ComponentModel.DataAnnotations;

namespace TrailRent.Catalog.Bll.Dtos;

public sealed record EquipmentListItemDto(int Id, string Sku, string Name, decimal DailyRate, int AvailableUnits, bool IsRentable, string Category);
public sealed record EquipmentDetailsDto(int Id, string Sku, string Name, decimal DailyRate, int AvailableUnits, string Category, decimal? WeightKg, string? Material, IReadOnlyList<string> Features, IReadOnlyList<string> PhotoUrls);
public sealed record CategoryDetailsDto(int Id, string Name, IReadOnlyList<EquipmentListItemDto> Equipment);

public sealed class CreateEquipmentRequest
{
    [Required, StringLength(32, MinimumLength = 3)] public string Sku { get; init; } = string.Empty;
    [Required, StringLength(160, MinimumLength = 3)] public string Name { get; init; } = string.Empty;
    [Range(typeof(decimal), "0.01", "999999.99")] public decimal DailyRate { get; init; }
    [Range(0, 10000)] public int AvailableUnits { get; init; }
    [Range(1, int.MaxValue)] public int CategoryId { get; init; }
}

public sealed class ChangeDailyRateRequest
{
    [Range(typeof(decimal), "0.01", "999999.99")] public decimal DailyRate { get; init; }
}

public sealed class StockChangeRequest
{
    [Range(1, 10000)] public int Units { get; init; }
}

public sealed class EquipmentQuery
{
    public string? Search { get; init; }
    [Range(1, int.MaxValue)] public int? CategoryId { get; init; }
    public bool? Rentable { get; init; }
    public string Sort { get; init; } = "rate";
    [Range(1, 100000)] public int Page { get; init; } = 1;
    [Range(1, 100)] public int PageSize { get; init; } = 20;
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize, int TotalPages, bool HasNext, bool HasPrevious);
