using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Dal.Specifications;

public sealed class EquipmentFilterSpecification : Specification<Equipment>
{
    public EquipmentFilterSpecification(string? search, int? categoryId, bool? rentable, string sort, int page, int pageSize, bool withPaging = true)
    {
        Criteria = e => (string.IsNullOrWhiteSpace(search) || e.Name.Contains(search) || e.Sku.Contains(search))
                     && (!categoryId.HasValue || e.CategoryId == categoryId)
                     && (!rentable.HasValue || e.IsRentable == rentable);
        Includes.Add(e => e.Category);
        OrderBy = sort.ToLowerInvariant() switch { "name" => e => e.Name, "sku" => e => e.Sku, _ => e => e.DailyRate };
        Descending = sort.Equals("rate_desc", StringComparison.OrdinalIgnoreCase);
        if (withPaging) { Skip = (page - 1) * pageSize; Take = pageSize; }
    }
}
