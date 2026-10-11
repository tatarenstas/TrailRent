using AutoMapper;
using TrailRent.Catalog.Bll.Dtos;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Bll.Mapping;

public sealed class CatalogProfile : Profile
{
    public CatalogProfile()
    {
        CreateMap<Equipment, EquipmentListItemDto>().ForCtorParam("Category", o => o.MapFrom(s => s.Category.Name));
        CreateMap<Equipment, EquipmentDetailsDto>()
            .ForCtorParam("Category", o => o.MapFrom(s => s.Category.Name))
            .ForCtorParam("WeightKg", o => o.MapFrom(s => s.Specs == null ? (decimal?)null : s.Specs.WeightKg))
            .ForCtorParam("Material", o => o.MapFrom(s => s.Specs == null ? null : s.Specs.Material))
            .ForCtorParam("Features", o => o.MapFrom(s => s.EquipmentFeatures.Select(f => f.Feature.Name).ToList()))
            .ForCtorParam("PhotoUrls", o => o.MapFrom(s => s.Photos.OrderBy(p => p.SortOrder).Select(p => p.Url).ToList()));
    }
}
