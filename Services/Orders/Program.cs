using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrailRent.Orders;
using TrailRent.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddTrailRentServiceDefaults();
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();
builder.Services.AddSingleton<OrdersSnapshot>();
builder.Services.AddSingleton<OrderMetrics>();
builder.Services.AddHealthChecks().AddSqlServer(
    builder.Configuration.GetConnectionString("OrdersDb") ?? "Server=localhost;Database=trailrent_orders;Trusted_Connection=True;",
    name: "orders-db", failureStatus: HealthStatus.Unhealthy, tags: ["database", "sql", "ready"], timeout: TimeSpan.FromSeconds(5));
var app = builder.Build();
app.MapGrpcHealthChecksService();
app.MapPost("/api/orders", async (CreateOrder request, OrdersSnapshot snapshot, OrderMetrics metrics, CancellationToken ct) =>
{
    var watch = Stopwatch.StartNew();
    try { await Task.Delay(1, ct); snapshot.IncrementAwaiting(); metrics.Created("awaiting_validation", request.PaymentMethod); metrics.Duration("create", true, watch.Elapsed.TotalMilliseconds); return Results.Created($"/api/orders/{Guid.NewGuid()}", new { status = "awaiting_validation" }); }
    catch { metrics.Duration("create", false, watch.Elapsed.TotalMilliseconds); throw; }
});
app.MapTrailRentHealthEndpoints();
app.Run();
record CreateOrder(string PaymentMethod);
