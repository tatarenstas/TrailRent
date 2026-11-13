# TrailRent — Lab 08

The platform uses RabbitMQ topic exchange `trailrent.events` for integration events. Catalog publishes `EquipmentRenamedIntegrationEvent` only after its `SaveChangesAsync` boundary succeeds. Orders and Reviews each have a durable independent queue, so one consumer never steals another consumer's copy.

Each queue is configured with `x-dead-letter-exchange = trailrent.events.dlx` and an isolated `.dlq`. Consumers use `BasicQos`, `AsyncEventingBasicConsumer`, `autoAck: false`; successful processing is acknowledged only after the unique `EventId` marker and local update are saved. Invalid JSON is immediately nacked without requeue. Transient failures in Orders republish with `x-retry-count`, acknowledge the original, then dead-letter after three attempts.

`EventId`, `OccurredAt` and `CorrelationId` are immutable record fields. Persistent camelCase JSON contains message id/type/correlation headers and W3C `traceparent`; the `TrailRent.Messaging` ActivitySource is exported by ServiceDefaults.

Run with `dotnet run --project Platform.AppHost` after installing the Aspire workload and starting Docker. The RabbitMQ Management UI appears in Aspire Dashboard resources.
