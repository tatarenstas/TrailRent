using TrailRent.Catalog.Bll.Dtos;

namespace TrailRent.Catalog.Bll.Services;

public interface IEquipmentService
{
    Task<PagedResult<EquipmentListItemDto>> GetPageAsync(EquipmentQuery query, CancellationToken cancellationToken = default);
    Task<EquipmentDetailsDto> GetDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<EquipmentDetailsDto> CreateAsync(CreateEquipmentRequest request, CancellationToken cancellationToken = default);
    Task ChangeDailyRateAsync(int id, ChangeDailyRateRequest request, CancellationToken cancellationToken = default);
    Task RemoveStockAsync(int id, StockChangeRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EquipmentListItemDto>> GetRentableAsync(CancellationToken cancellationToken = default);
}
