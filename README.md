# TrailRent — Lab 07

Resilience, business metrics and comprehensive health checks for the TrailRent equipment-rental platform.

| Client | Criticality | Retry / breaker / timeout rationale |
|---|---|---|
| Catalog | Critical | A rental card cannot be composed without equipment details, so it uses five idempotent retries, a 30% breaker threshold and a 25-second total budget. |
| Reviews | Optional | Reviews enrich a card only. One retry, a 2-second attempt timeout and a tolerant 70% breaker let Aggregator return a partial response quickly. |

`Platform.ServiceDefaults` configures exactly one standard HTTP resilience handler for factory clients and exports the `Polly`, `Orders.Api`, `Catalog.Api`, `Reviews.Api`, `Catalog.Cache` and `TrailRent.Messaging` meters. gRPC retries are configured per channel because gRPC status failures are carried in trailers.

Every service exposes `/health/live`, `/health/ready` and `/health`. Readiness returns JSON details. SQL/Mongo checks are unhealthy when unavailable; Redis is deliberately `Degraded` because cache failure falls back to the source of truth.

Run a service with `dotnet run --project Services/Orders`. Running `Platform.AppHost` requires the .NET Aspire workload and Docker.
