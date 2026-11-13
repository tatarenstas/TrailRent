using System.Diagnostics;
using TrailRent.Messaging;
using TrailRent.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddTrailRentServiceDefaults();
builder.AddRabbitMQClient("messaging"); // Aspire.RabbitMQ.Client registers IConnection and its health check.
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
var app = builder.Build();

app.MapPut("/api/equipment/{id:guid}/name", async (Guid id, RenameEquipment request, IEventPublisher publisher, CancellationToken ct) =>
{
    var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
    // Transaction boundary: SaveChangesAsync succeeds before the integration event leaves this service.
    await SaveChangesAsync(ct);
    var changed = new EquipmentRenamedIntegrationEvent(IntegrationEvent.NewId(), DateTimeOffset.UtcNow, correlationId, id, request.PreviousName, request.NewName);
    await publisher.PublishAsync(changed, ct);
    return Results.NoContent();
});
app.Run();

static Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;
record RenameEquipment(string PreviousName, string NewName);
