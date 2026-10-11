using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using TrailRent.Reviews.Domain.Entities;
namespace TrailRent.Reviews.Infrastructure.Persistence;
public sealed class IndexCreationService(MongoDbContext context) : IHostedService
{
    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
    public Task StartAsync(CancellationToken ct)
    {
        var keys = Builders<Review>.IndexKeys;
        return context.Reviews.Indexes.CreateManyAsync([
            new CreateIndexModel<Review>(keys.Ascending(r => r.Product.ProductId).Descending(r => r.CreatedAt), new CreateIndexOptions { Name = "product_recent" }),
            new CreateIndexModel<Review>(keys.Text(r => r.Text), new CreateIndexOptions { Name = "review_text" }),
            new CreateIndexModel<Review>(keys.Ascending(r => r.Product.ProductId).Ascending("author.userId"), new CreateIndexOptions { Name = "unique_author_product", Unique = true })], ct);
    }
}
