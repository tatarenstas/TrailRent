using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Specifications;

// A second specification: required separately from the filtered catalog query.
public sealed class RentableEquipmentSpecification : Specification<Equipment>
{
    public RentableEquipmentSpecification()
    {
        Criteria = e => e.IsRentable && e.AvailableUnits > 0;
        OrderBy = e => e.Name;
        Includes.Add(e => e.Category);
    }
}
