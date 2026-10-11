using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using TrailRent.Reviews.Application;
using TrailRent.Reviews.Application.Dtos;
using TrailRent.Reviews.Domain.Entities;
namespace TrailRent.Reviews.Infrastructure.Persistence;
public sealed class ReviewQueries(MongoDbContext context, IMapper mapper) : IReviewQueries
{
    private readonly IMongoCollection<Review> _reviews = context.Reviews;
    public async Task<ReviewDto?> GetByIdAsync(string id, CancellationToken ct = default) => mapper.Map<ReviewDto>(await _reviews.Find(r => r.Id == ObjectId.Parse(id)).FirstOrDefaultAsync(ct));
    public async Task<PagedResult<ReviewDto>> GetListAsync(long? productId, string? text, int page, int pageSize, CancellationToken ct = default)
    {
        var filter = Builders<Review>.Filter.Empty;
        if (productId.HasValue) filter &= Builders<Review>.Filter.Eq(r => r.Product.ProductId, productId.Value);
        if (!string.IsNullOrWhiteSpace(text)) filter &= Builders<Review>.Filter.Text(text);
        var count = await _reviews.CountDocumentsAsync(filter, cancellationToken: ct);
        var reviews = await _reviews.Find(filter).SortByDescending(r => r.CreatedAt).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync(ct);
        var totalPages = Math.Max(1, (int)Math.Ceiling(count / (double)pageSize));
        return new(mapper.Map<IReadOnlyList<ReviewDto>>(reviews), count, page, pageSize, totalPages, page < totalPages);
    }
}
