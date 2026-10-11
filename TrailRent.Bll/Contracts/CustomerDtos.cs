using System.ComponentModel.DataAnnotations;

namespace TrailRent.Bll.Contracts;

public sealed record CustomerDto(long Id, string FirstName, string LastName, string Email, string? PhoneNumber, bool IsActive);

public sealed class UpsertCustomerRequest
{
    [Required, StringLength(50)] public string FirstName { get; init; } = string.Empty;
    [Required, StringLength(50)] public string LastName { get; init; } = string.Empty;
    [Required, EmailAddress, StringLength(100)] public string Email { get; init; } = string.Empty;
    [StringLength(20)] public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; } = true;
}
