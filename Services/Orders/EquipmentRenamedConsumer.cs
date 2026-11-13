using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TrailRent.Messaging;

namespace TrailRent.Orders;

public sealed class EquipmentRenamedConsumer(IConnection connection, IServiceScopeFactory scopes, ILogger<EquipmentRenamedConsumer> logger) : BackgroundService
{
    private const string Queue = "orders.equipment-renamed";
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await RabbitMqTopology.DeclareAsync(channel, Queue, "equipment.renamed", stoppingToken);
        await channel.BasicQosAsync(0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, delivery) => await HandleAsync(channel, delivery, stoppingToken);
        await channel.BasicConsumeAsync(Queue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleAsync(IChannel channel, BasicDeliverEventArgs delivery, CancellationToken ct)
    {
        var correlationId = Header(delivery, MessagingTelemetry.CorrelationHeader)?.ToString() ?? "missing";
        using var activity = MessagingTelemetry.StartConsumerActivity("orders consume equipment.renamed", delivery.BasicProperties, Queue);
        try
        {
            var item = JsonSerializer.Deserialize<EquipmentRenamedIntegrationEvent>(delivery.Body.Span) ?? throw new JsonException("Invalid event JSON");
            using var scope = scopes.CreateScope();
            var processed = scope.ServiceProvider.GetRequiredService<ProcessedEventStore>();
            if (await processed.TryInsertInTransactionAsync(item.EventId, ct)) { scope.ServiceProvider.GetRequiredService<LocalEquipmentCatalog>().Rename(item.EquipmentId, item.NewName); await processed.SaveChangesAsync(ct); }
            await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, cancellationToken: ct);
            logger.LogInformation("Ack event {EventId} {EventType} EquipmentId={EquipmentId} CorrelationId={CorrelationId}", item.EventId, nameof(EquipmentRenamedIntegrationEvent), item.EquipmentId, correlationId);
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "Permanent event error; Nack to DLQ EventId={EventId} CorrelationId={CorrelationId}", delivery.BasicProperties.MessageId, correlationId);
            await channel.BasicNackAsync(delivery.DeliveryTag, multiple: false, requeue: false, cancellationToken: ct);
        }
        catch (Exception exception) { await RetryOrDeadLetterAsync(channel, delivery, exception, correlationId, ct); }
    }

    private async Task RetryOrDeadLetterAsync(IChannel channel, BasicDeliverEventArgs delivery, Exception exception, string correlationId, CancellationToken ct)
    {
        var retries = Header(delivery, MessagingTelemetry.RetryHeader) is byte[] bytes ? int.Parse(System.Text.Encoding.UTF8.GetString(bytes)) : 0;
        if (retries >= 3) { logger.LogError(exception, "Retries exhausted; Nack to DLQ EventId={EventId} CorrelationId={CorrelationId}", delivery.BasicProperties.MessageId, correlationId); await channel.BasicNackAsync(delivery.DeliveryTag, false, false, ct); return; }
        var properties = new BasicProperties { Persistent = true, ContentType = delivery.BasicProperties.ContentType, MessageId = delivery.BasicProperties.MessageId, Type = delivery.BasicProperties.Type, Headers = delivery.BasicProperties.Headers is null ? [] : new Dictionary<string, object?>(delivery.BasicProperties.Headers) };
        properties.Headers[MessagingTelemetry.RetryHeader] = System.Text.Encoding.UTF8.GetBytes((retries + 1).ToString());
        await channel.BasicPublishAsync(RabbitMqTopology.EventsExchange, "equipment.renamed", true, properties, delivery.Body, ct);
        await channel.BasicAckAsync(delivery.DeliveryTag, false, ct);
        logger.LogWarning(exception, "Republished event {EventId}; x-retry-count={RetryCount} CorrelationId={CorrelationId}", properties.MessageId, retries + 1, correlationId);
    }

    private static object? Header(BasicDeliverEventArgs delivery, string key) => delivery.BasicProperties.Headers is { } headers && headers.TryGetValue(key, out var value) ? value : null;
}
