using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace TrailRent.Catalog.Api.Infrastructure;

public static class ValidationExtensions
{
    public static async Task<ValidationProblemDetails?> ValidateAsync<T>(this IValidator<T> validator, T model, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(model, cancellationToken);
        if (result.IsValid) return null;
        return new ValidationProblemDetails(result.Errors.GroupBy(x => x.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));
    }
}
