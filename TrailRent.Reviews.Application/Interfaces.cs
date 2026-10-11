using TrailRent.Reviews.Application.Dtos;
namespace TrailRent.Reviews.Application;
public interface IReviewQueries
{
    Task<ReviewDto?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<PagedResult<ReviewDto>> GetListAsync(long? productId, string? text, int page, int pageSize, CancellationToken ct = default);
}
