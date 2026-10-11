using MongoDB.Bson;
using MongoDB.Driver;
using TrailRent.Reviews.Domain.Common;
using TrailRent.Reviews.Domain.Entities;
using TrailRent.Reviews.Domain.Interfaces;
namespace TrailRent.Reviews.Infrastructure.Persistence;
public sealed class ReviewRepository(MongoDbContext context) : IReviewRepository
{
    private readonly IMongoCollection<Review> _reviews = context.Reviews;
    public async Task<Review?> GetByIdAsync(ObjectId id, CancellationToken ct = default) => await _reviews.Find(r => r.Id == id).FirstOrDefaultAsync(ct);
    public Task<bool> ExistsAsync(long productId, string authorId, CancellationToken ct = default) => _reviews.Find(Builders<Review>.Filter.Eq(r => r.Product.ProductId, productId) & Builders<Review>.Filter.Eq("author.userId", authorId)).AnyAsync(ct);
    public Task AddAsync(Review review, CancellationToken ct = default) => _reviews.InsertOneAsync(review, cancellationToken: ct);
    public async Task ReplaceAsync(Review review, int expectedVersion, CancellationToken ct = default)
    { review.BumpVersion(); var result = await _reviews.ReplaceOneAsync(r => r.Id == review.Id && r.Version == expectedVersion, review, cancellationToken: ct); if (result.MatchedCount == 0) throw new ConflictException("Review was changed by another request."); }
    public async Task DeleteAsync(ObjectId id, int expectedVersion, CancellationToken ct = default)
    { var result = await _reviews.DeleteOneAsync(r => r.Id == id && r.Version == expectedVersion, ct); if (result.DeletedCount == 0) throw new ConflictException("Review was changed or already deleted."); }
}
