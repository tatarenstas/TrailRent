using FluentValidation;
using TrailRent.Catalog.Bll.Dtos;

namespace TrailRent.Catalog.Bll.Validators;

public sealed class EquipmentQueryValidator : AbstractValidator<EquipmentQuery>
{
    private static readonly string[] Sorts = ["rate", "rate_desc", "name", "sku"];
    public EquipmentQueryValidator() => RuleFor(x => x.Sort).Must(Sorts.Contains).WithMessage("Sort must be rate, rate_desc, name or sku.");
}
