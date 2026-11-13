using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace TrailRent.Messaging;

public interface IEventPublisher { Task PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default); }

public sealed class RabbitMqEventPublisher(IConnection connection, ILogger<RabbitMqEventPublisher> logger) : IEventPublisher
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.ExchangeDeclareAsync(RabbitMqTopology.EventsExchange, ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = integrationEvent.EventId.ToString(),
            Type = integrationEvent.GetType().Name,
            Headers = new Dictionary<string, object?> { [MessagingTelemetry.CorrelationHeader] = integrationEvent.CorrelationId }
        };
        MessagingTelemetry.InjectTraceContext(properties);
        var body = JsonSerializer.SerializeToUtf8Bytes(integrationEvent, integrationEvent.GetType(), Json);
        await channel.BasicPublishAsync(RabbitMqTopology.EventsExchange, "equipment.renamed", mandatory: true, basicProperties: properties, body: body, cancellationToken: cancellationToken);
        logger.LogInformation("Published integration event {EventId} {EventType} CorrelationId={CorrelationId}", integrationEvent.EventId, properties.Type, integrationEvent.CorrelationId);
    }
}
