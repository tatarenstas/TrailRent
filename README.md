# TrailRent Platform — Lab 05

An Aspire-orchestrated TrailRent platform. Each lab branch is independent; this branch introduces the platform shell and lightweight contract-compatible service adapters for Orders, Catalog and Reviews.

```text
Gateway (port 5000) → Orders / Catalog / Reviews
                 └──→ Aggregator ──parallel HTTP──→ three services
AppHost → SQL Server + MongoDB + every service
```

## Components

- `Platform.AppHost` starts databases, services, Aggregator and YARP Gateway; each database has `WithDataVolume()`.
- `Platform.ServiceDefaults` supplies OpenTelemetry, health endpoints, resilient service discovery HTTP clients, Serilog and propagation of `X-Correlation-Id`.
- `Platform.ApiGateway` uses YARP logical destinations (`orders-api`, `catalog-api`, `reviews-api`, `aggregator`) and publishes port 5000.
- `Platform.Aggregator` uses typed HTTP clients, `Task.WhenAll`, cancellation propagation and warnings for partial outages.

## Run

```bash
dotnet user-secrets set "Parameters:sql-password" "TrailRent!2026" --project Platform.AppHost
dotnet run --project Platform.AppHost
```

Open the Aspire Dashboard URL printed by AppHost. The gateway examples are:

```bash
curl -i http://localhost:5000/api/orders/1
curl -i 'http://localhost:5000/api/products?ids=101,205'
curl -i http://localhost:5000/api/reviews?productId=101
curl -i http://localhost:5000/api/aggregator/orders/1/full
```

Every response includes `X-Correlation-Id`. To demonstrate resilience, stop `reviews-api` in Dashboard and repeat the aggregator request: it returns order/products with a `warnings` entry instead of HTTP 500.

## Verification

Run `dotnet build TrailRent.Platform.sln`. See [docs/lab-05/checklist.md](docs/lab-05/checklist.md) for required Dashboard evidence.
