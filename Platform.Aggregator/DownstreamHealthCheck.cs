using Grpc.Core;
using Grpc.Health.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TrailRent.Aggregator;
public sealed class CatalogGrpcHealthCheck(Health.HealthClient client, ILogger<CatalogGrpcHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try { var reply = await client.CheckAsync(new HealthCheckRequest(), deadline: DateTime.UtcNow.AddSeconds(2), cancellationToken: cancellationToken); return reply.Status == HealthCheckResponse.Types.ServingStatus.Serving ? HealthCheckResult.Healthy("catalog-api is serving") : HealthCheckResult.Unhealthy($"catalog-api status: {reply.Status}"); }
        catch (RpcException exception) { logger.LogWarning(exception, "catalog-api health check failed"); return HealthCheckResult.Unhealthy("catalog-api is unavailable", exception); }
    }
}
public sealed class CatalogClient(HttpClient http) { public HttpClient Http { get; } = http; }
public sealed class ReviewsClient(HttpClient http) { public HttpClient Http { get; } = http; }
