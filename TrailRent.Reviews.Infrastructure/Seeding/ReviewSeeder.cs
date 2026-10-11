using TrailRent.Reviews.Domain.Entities;
using TrailRent.Reviews.Domain.ValueObjects;
using TrailRent.Reviews.Infrastructure.Persistence;
namespace TrailRent.Reviews.Infrastructure.Seeding;
public sealed class ReviewSeeder(MongoDbContext context) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken ct = default)
    { if (await context.Reviews.EstimatedDocumentCountAsync(cancellationToken: ct) > 0) return; var first = new Review(new ProductReference(101, "Alpine two-person tent"), new Author("u-olena", "Олена"), 5, "Легкий намет, витримав сильний дощ."); first.AddComment(new Author("u-admin", "TrailRent"), "Дякуємо за відгук!"); var reviews = new[] { first, new Review(new ProductReference(101, "Alpine two-person tent"), new Author("u-ivan", "Іван"), 4, "Зручний, але потрібні кращі кілочки."), new Review(new ProductReference(205, "Carbon trekking poles"), new Author("u-marta", "Марта"), 5, "Дуже легкі для довгого маршруту.") }; await context.Reviews.InsertManyAsync(reviews, cancellationToken: ct); }
}
