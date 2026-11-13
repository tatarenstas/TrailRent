using System.Diagnostics;
using RabbitMQ.Client;

namespace TrailRent.Messaging;

public abstract record IntegrationEvent(Guid EventId, DateTimeOffset OccurredAt, string CorrelationId)
{
    public static Guid NewId() => Guid.NewGuid();
}

// Past tense and self-contained: consumers need no synchronous callback to Catalog.
public sealed record EquipmentRenamedIntegrationEvent(Guid EventId, DateTimeOffset OccurredAt, string CorrelationId,
    Guid EquipmentId, string PreviousName, string NewName) : IntegrationEvent(EventId, OccurredAt, CorrelationId);

public static class MessagingTelemetry
{
    public static readonly ActivitySource Source = new("TrailRent.Messaging");
    public const string CorrelationHeader = "x-correlation-id";
    public const string RetryHeader = "x-retry-count";

    // Inject W3C traceparent into broker headers on publish.
    public static void InjectTraceContext(BasicProperties properties)
    {
        if (Activity.Current?.Id is { } traceparent)
        {
            properties.Headers ??= new Dictionary<string, object?>();
            properties.Headers["traceparent"] = traceparent;
        }
    }

    // Extract W3C traceparent before creating the Consumer activity.
    public static Activity? StartConsumerActivity(string name, IReadOnlyBasicProperties properties, string destination)
    {
        var parent = default(ActivityContext);
        if (properties.Headers is { } headers && headers.TryGetValue("traceparent", out var header))
            ActivityContext.TryParse(header?.ToString(), null, out parent);
        var activity = Source.StartActivity(name, ActivityKind.Consumer, parent);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", destination);
        activity?.SetTag("messaging.operation", "process");
        return activity;
    }
}
