using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TrailRent.Reviews.Domain.Entities;
namespace TrailRent.Reviews.Infrastructure.Persistence;
public sealed class MongoDbContext
{
    public MongoDbContext(IOptions<MongoDbOptions> options) { Client = new MongoClient(options.Value.ConnectionString); Database = Client.GetDatabase(options.Value.DatabaseName); }
    public MongoClient Client { get; }
    public IMongoDatabase Database { get; }
    // The collection name follows Project #3 of Lab 01.
    public IMongoCollection<Review> Reviews => Database.GetCollection<Review>("equipment_reviews");
}
