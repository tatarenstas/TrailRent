# Lab 05 checklist

- AppHost owns SQL Server, MongoDB, three services, Aggregator and Gateway. SQL password is an Aspire secret parameter; both stores use persistent data volumes.
- All HTTP services reference `Platform.ServiceDefaults`, call `AddServiceDefaults`, `UseCorrelationId` and `MapDefaultEndpoints`.
- Correlation middleware returns and forwards `X-Correlation-Id`; Serilog enriches each request with it plus service/environment/machine/thread details.
- Gateway uses YARP routes with service-discovery addresses. Aggregator uses three typed clients and `Task.WhenAll`; each remote call is wrapped to preserve partial data on an outage.
- OpenTelemetry instruments incoming ASP.NET requests, outgoing HTTP and SqlClient. ServiceDefaults exports through OTLP when Aspire config supplies an endpoint.

After starting AppHost, capture Dashboard resources in Running state, the trace for `/api/aggregator/orders/1/full`, and structured logs filtered by the returned `X-Correlation-Id`. Then stop `reviews-api`, repeat the request and show the `warnings` field.
