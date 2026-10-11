using TrailRent.Reviews.Domain.Common;
namespace TrailRent.Reviews.Domain.ValueObjects;
public sealed class Author : ValueObject
{
    public string UserId { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    private Author() { }
    public Author(string userId, string displayName)
    { if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(displayName)) throw new DomainException("Author identity and name are required."); UserId = userId; DisplayName = displayName; }
    protected override IEnumerable<object?> GetEqualityComponents() { yield return UserId; yield return DisplayName; }
}
