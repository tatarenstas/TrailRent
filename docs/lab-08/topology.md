# Broker topology and data-copy policy

`trailrent.events` is a durable topic exchange. `orders.equipment-renamed` and `reviews.equipment-renamed` bind the `equipment.renamed` routing key and declare `x-dead-letter-exchange: trailrent.events.dlx`; each has a durable `*.dlq` bound to the DLX.

Orders update only their local equipment reference (name/SKU); prices already written to an order line are immutable. Reviews update the denormalized equipment name across review documents. Each handler writes a `ProcessedIntegrationEvent` with `EventId` as its unique key in the same transaction as its projection update, making delivery at-least-once safe.
