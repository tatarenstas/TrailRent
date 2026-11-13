using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrailRent.Catalog;
using TrailRent.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddTrailRentServiceDefaults();
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();
builder.Services.AddSingleton<CatalogMetrics>();
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("CatalogDb") ?? "Server=localhost;Database=trailrent_catalog;Trusted_Connection=True;", name: "catalog-db", failureStatus: HealthStatus.Unhealthy, tags: ["database", "sql", "ready"], timeout: TimeSpan.FromSeconds(5))
    .AddRedis(builder.Configuration.GetConnectionString("redis") ?? "localhost:6379", name: "redis-cache", failureStatus: HealthStatus.Degraded, tags: ["cache", "redis", "ready"], timeout: TimeSpan.FromSeconds(3));
var app = builder.Build();
app.MapGrpcHealthChecksService();
app.MapPut("/api/products/{id:guid}", (Guid id, CatalogMetrics metrics) => { metrics.Updated("success"); metrics.Duration("update", true, 1); return Results.NoContent(); });
app.MapTrailRentHealthEndpoints();
app.Run();
