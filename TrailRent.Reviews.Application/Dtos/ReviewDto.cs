namespace TrailRent.Reviews.Application.Dtos;
public sealed record ReviewDto(string Id, long ProductId, string ProductName, string AuthorName, int Rating, string Text, int CommentsCount, int Version, DateTime CreatedAt);
