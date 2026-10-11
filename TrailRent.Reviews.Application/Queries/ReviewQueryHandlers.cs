using MediatR;
using TrailRent.Reviews.Application.Dtos;
using TrailRent.Reviews.Domain.Common;
namespace TrailRent.Reviews.Application.Queries;
public sealed class GetReviewByIdQueryHandler(IReviewQueries queries) : IRequestHandler<GetReviewByIdQuery, ReviewDto>
{ public async Task<ReviewDto> Handle(GetReviewByIdQuery request, CancellationToken ct) => await queries.GetByIdAsync(request.Id, ct) ?? throw new NotFoundException("Review", request.Id); }
public sealed class GetReviewsListQueryHandler(IReviewQueries queries) : IRequestHandler<GetReviewsListQuery, PagedResult<ReviewDto>>
{ public Task<PagedResult<ReviewDto>> Handle(GetReviewsListQuery request, CancellationToken ct) => queries.GetListAsync(request.ProductId, request.Text, Math.Max(1, request.Page), Math.Clamp(request.PageSize, 1, 100), ct); }
