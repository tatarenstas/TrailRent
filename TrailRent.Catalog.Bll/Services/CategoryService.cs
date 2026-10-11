using AutoMapper;
using TrailRent.Catalog.Bll.Dtos;
using TrailRent.Catalog.Dal.Repositories;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Bll.Services;

public sealed class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    public async Task<CategoryDetailsDto> GetWithEquipmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.Categories.GetWithEquipmentAsync(id, cancellationToken) ?? throw new EntityNotFoundException("EquipmentCategory", id);
        return new(category.Id, category.Name, mapper.Map<IReadOnlyList<EquipmentListItemDto>>(category.Equipment));
    }

    public async Task<IReadOnlyList<EquipmentListItemDto>> GetFeatureEquipmentAsync(string featureName, DateTime since, CancellationToken cancellationToken = default) =>
        mapper.Map<IReadOnlyList<EquipmentListItemDto>>(await unitOfWork.EquipmentFeatures.GetEquipmentByFeatureSinceAsync(featureName, since, cancellationToken));
}
