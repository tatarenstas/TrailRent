using AutoMapper;
using TrailRent.Bll.Contracts;
using TrailRent.Domain.Entities;

namespace TrailRent.Bll.Mapping;

public sealed class TrailRentProfile : Profile
{
    public TrailRentProfile()
    {
        CreateMap<Customer, CustomerDto>();
        CreateMap<UpsertCustomerRequest, Customer>();
        CreateMap<RentalItem, RentalItemDto>();
        CreateMap<Rental, RentalDto>();
    }
}
