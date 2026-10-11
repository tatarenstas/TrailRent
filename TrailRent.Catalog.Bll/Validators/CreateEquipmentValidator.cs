using FluentValidation;
using TrailRent.Catalog.Bll.Dtos;

namespace TrailRent.Catalog.Bll.Validators;

public sealed class CreateEquipmentValidator : AbstractValidator<CreateEquipmentRequest>
{
    public CreateEquipmentValidator()
    {
        RuleFor(x => x.Sku).Matches("^[A-Z0-9-]+$").WithMessage("SKU must contain uppercase letters, numbers and hyphens only.");
        RuleFor(x => x.Name).Must(name => !name.Contains("test", StringComparison.OrdinalIgnoreCase)).WithMessage("Name must be a real catalog name.");
    }
}
