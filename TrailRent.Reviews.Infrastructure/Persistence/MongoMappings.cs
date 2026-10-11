using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using TrailRent.Reviews.Domain.Entities;
using TrailRent.Reviews.Domain.ValueObjects;
namespace TrailRent.Reviews.Infrastructure.Persistence;
public static class MongoMappings
{
    public static void Register()
    {
        ConventionRegistry.Register("TrailRentCamelCase", new ConventionPack { new CamelCaseElementNameConvention(), new IgnoreExtraElementsConvention(true) }, _ => true);
        if (!BsonClassMap.IsClassMapRegistered(typeof(Author))) BsonClassMap.RegisterClassMap<Author>(cm => { cm.AutoMap(); cm.MapCreator(a => new Author(a.UserId, a.DisplayName)); });
        if (!BsonClassMap.IsClassMapRegistered(typeof(ProductReference))) BsonClassMap.RegisterClassMap<ProductReference>(cm => { cm.AutoMap(); cm.MapCreator(p => new ProductReference(p.ProductId, p.ProductName)); });
        if (!BsonClassMap.IsClassMapRegistered(typeof(Comment))) BsonClassMap.RegisterClassMap<Comment>(cm => cm.AutoMap());
        if (!BsonClassMap.IsClassMapRegistered(typeof(Review))) BsonClassMap.RegisterClassMap<Review>(cm => { cm.AutoMap(); cm.UnmapProperty(r => r.Comments); cm.MapIdProperty(r => r.Id); cm.MapField("_comments").SetElementName("comments"); });
    }
}
