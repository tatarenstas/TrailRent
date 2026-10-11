using MongoDB.Bson;
using TrailRent.Reviews.Domain.Common;
using TrailRent.Reviews.Domain.ValueObjects;
namespace TrailRent.Reviews.Domain.Entities;
public sealed class Comment
{
    public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();
    public Author Author { get; private set; } = null!;
    public string Text { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    private Comment() { }
    public Comment(Author author, string text)
    { if (string.IsNullOrWhiteSpace(text)) throw new DomainException("Comment text is required."); Author = author; Text = text; CreatedAt = DateTime.UtcNow; }
}
