using MongoDB.Bson;
using TrailRent.Reviews.Domain.Entities;
namespace TrailRent.Reviews.Domain.Interfaces;
public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(ObjectId id, CancellationToken ct = default);
    Task<bool> ExistsAsync(long productId, string authorId, CancellationToken ct = default);
    Task AddAsync(Review review, CancellationToken ct = default);
    Task ReplaceAsync(Review review, int expectedVersion, CancellationToken ct = default);
    Task DeleteAsync(ObjectId id, int expectedVersion, CancellationToken ct = default);
}
