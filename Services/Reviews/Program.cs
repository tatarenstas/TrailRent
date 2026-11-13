using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrailRent.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddTrailRentServiceDefaults();
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();
builder.Services.AddHealthChecks().AddMongoDb(
    builder.Configuration.GetConnectionString("ReviewsDb") ?? "mongodb://localhost:27017/trailrent_reviews",
    name: "reviews-db", failureStatus: HealthStatus.Unhealthy, tags: ["database", "mongodb", "ready"], timeout: TimeSpan.FromSeconds(5));
var app = builder.Build();
app.MapGrpcHealthChecksService();
app.MapGet("/api/reviews/{productId:guid}", (Guid productId) => Results.Ok(Array.Empty<object>()));
app.MapTrailRentHealthEndpoints();
app.Run();
