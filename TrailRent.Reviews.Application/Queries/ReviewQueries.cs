using TrailRent.Reviews.Application.Dtos;
namespace TrailRent.Reviews.Application.Queries;
public sealed record GetReviewByIdQuery(string Id) : IQuery<ReviewDto>;
public sealed record GetReviewsListQuery(long? ProductId, string? Text, int Page = 1, int PageSize = 20) : IQuery<PagedResult<ReviewDto>>;
