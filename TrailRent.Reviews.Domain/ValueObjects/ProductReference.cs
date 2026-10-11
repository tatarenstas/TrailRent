using TrailRent.Reviews.Domain.Common;
namespace TrailRent.Reviews.Domain.ValueObjects;
public sealed class ProductReference : ValueObject
{
    public long ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    private ProductReference() { }
    public ProductReference(long productId, string productName)
    { if (productId <= 0 || string.IsNullOrWhiteSpace(productName)) throw new DomainException("Valid product data is required."); ProductId = productId; ProductName = productName; }
    protected override IEnumerable<object?> GetEqualityComponents() { yield return ProductId; yield return ProductName; }
}
