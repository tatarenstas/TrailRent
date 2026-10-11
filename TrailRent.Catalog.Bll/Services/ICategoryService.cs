using TrailRent.Catalog.Bll.Dtos;

namespace TrailRent.Catalog.Bll.Services;

public interface ICategoryService
{
    Task<CategoryDetailsDto> GetWithEquipmentAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EquipmentListItemDto>> GetFeatureEquipmentAsync(string featureName, DateTime since, CancellationToken cancellationToken = default);
}
