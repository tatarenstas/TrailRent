using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrailRent.Reviews.Application;
using TrailRent.Reviews.Domain.Interfaces;
using TrailRent.Reviews.Infrastructure.Persistence;
using TrailRent.Reviews.Infrastructure.Seeding;
namespace TrailRent.Reviews.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    { MongoMappings.Register(); services.Configure<MongoDbOptions>(config.GetSection("MongoDb")); services.AddSingleton<MongoDbContext>(); services.AddScoped<IReviewRepository, ReviewRepository>(); services.AddScoped<IReviewQueries, ReviewQueries>(); services.AddScoped<IDataSeeder, ReviewSeeder>(); services.AddHostedService<IndexCreationService>(); return services; }
}
