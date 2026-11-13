using RabbitMQ.Client;

namespace TrailRent.Messaging;

public static class RabbitMqTopology
{
    public const string EventsExchange = "trailrent.events";
    public const string DeadLetterExchange = "trailrent.events.dlx";

    public static async Task DeclareAsync(IChannel channel, string queue, string routingKey, CancellationToken ct)
    {
        // Durable topic exchange, isolated consumer queue and a per-queue dead-letter route.
        await channel.ExchangeDeclareAsync(EventsExchange, ExchangeType.Topic, durable: true, cancellationToken: ct);
        await channel.ExchangeDeclareAsync(DeadLetterExchange, ExchangeType.Topic, durable: true, cancellationToken: ct);
        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?> { ["x-dead-letter-exchange"] = DeadLetterExchange }, cancellationToken: ct);
        await channel.QueueBindAsync(queue, EventsExchange, routingKey, cancellationToken: ct);
        var deadQueue = $"{queue}.dlq";
        await channel.QueueDeclareAsync(deadQueue, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
        await channel.QueueBindAsync(deadQueue, DeadLetterExchange, routingKey, cancellationToken: ct);
    }
}
