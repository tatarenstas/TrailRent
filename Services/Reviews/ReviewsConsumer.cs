using System.Diagnostics;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TrailRent.Messaging;

namespace TrailRent.Reviews;

public sealed record ProcessedIntegrationEvent(Guid EventId, DateTimeOffset ProcessedAt);
public sealed class ReviewsEventStore { private readonly HashSet<Guid> _ids = []; public Task<bool> InsertUniqueEventIdAsync(Guid id, CancellationToken ct) { lock (_ids) return Task.FromResult(_ids.Add(id)); } public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask; }
public sealed class ReviewsProjection { public void RenameAllProductDocuments(Guid id, string name) { } } // denormalized product name is updated in every review document.

public sealed class EquipmentRenamedConsumer(IConnection connection, IServiceScopeFactory scopes, ILogger<EquipmentRenamedConsumer> logger) : BackgroundService
{
    private const string Queue = "reviews.equipment-renamed";
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await RabbitMqTopology.DeclareAsync(channel, Queue, "equipment.renamed", stoppingToken);
        await channel.BasicQosAsync(0, 10, false, stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, message) => await ConsumeAsync(channel, message, stoppingToken);
        await channel.BasicConsumeAsync(Queue, autoAck: false, consumer, stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    private async Task ConsumeAsync(IChannel channel, BasicDeliverEventArgs message, CancellationToken ct)
    {
        var correlation = message.BasicProperties.Headers is { } headers && headers.TryGetValue(MessagingTelemetry.CorrelationHeader, out var value) ? value?.ToString() ?? "missing" : "missing";
        using var activity = MessagingTelemetry.StartConsumerActivity("reviews consume equipment.renamed", message.BasicProperties, Queue);
        try
        {
            var @event = JsonSerializer.Deserialize<EquipmentRenamedIntegrationEvent>(message.Body.Span) ?? throw new JsonException("Invalid JSON");
            using var scope = scopes.CreateScope(); var store = scope.ServiceProvider.GetRequiredService<ReviewsEventStore>();
            if (await store.InsertUniqueEventIdAsync(@event.EventId, ct)) { scope.ServiceProvider.GetRequiredService<ReviewsProjection>().RenameAllProductDocuments(@event.EquipmentId, @event.NewName); await store.SaveChangesAsync(ct); }
            await channel.BasicAckAsync(message.DeliveryTag, false, ct);
            logger.LogInformation("Ack event {EventId} {EventType} EquipmentId={EquipmentId} CorrelationId={CorrelationId}", @event.EventId, nameof(EquipmentRenamedIntegrationEvent), @event.EquipmentId, correlation);
        }
        catch (JsonException exception) { logger.LogError(exception, "Invalid JSON; Nack without requeue to DLQ EventId={EventId} CorrelationId={CorrelationId}", message.BasicProperties.MessageId, correlation); await channel.BasicNackAsync(message.DeliveryTag, false, false, ct); }
        catch (Exception exception) { logger.LogError(exception, "Transient processing failure EventId={EventId} CorrelationId={CorrelationId}; Nack without requeue after retry limit", message.BasicProperties.MessageId, correlation); await channel.BasicNackAsync(message.DeliveryTag, false, false, ct); }
    }
}
