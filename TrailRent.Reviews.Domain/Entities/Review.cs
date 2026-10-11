using TrailRent.Reviews.Domain.Common;
using TrailRent.Reviews.Domain.ValueObjects;
namespace TrailRent.Reviews.Domain.Entities;
public sealed class Review : BaseEntity
{
    private readonly List<Comment> _comments = [];
    public ProductReference Product { get; private set; } = null!;
    public Author Author { get; private set; } = null!;
    public int Rating { get; private set; }
    public string Text { get; private set; } = null!;
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    private Review() { }
    public Review(ProductReference product, Author author, int rating, string text)
    { Product = product; Author = author; SetContent(rating, text); }
    public void Update(int rating, string text) { SetContent(rating, text); Touch(); }
    public void AddComment(Author author, string text) { _comments.Add(new Comment(author, text)); Touch(); }
    private void SetContent(int rating, string text)
    { if (rating is < 1 or > 5) throw new DomainException("Rating must be between 1 and 5."); if (string.IsNullOrWhiteSpace(text) || text.Length > 2000) throw new DomainException("Review text is required and limited to 2000 characters."); Rating = rating; Text = text; }
}
