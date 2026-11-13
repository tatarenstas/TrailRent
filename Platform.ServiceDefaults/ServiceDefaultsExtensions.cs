using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace TrailRent.ServiceDefaults;

public static class ServiceDefaultsExtensions
{
    public static TBuilder AddTrailRentServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHttpClient();
        builder.Services.ConfigureHttpClientDefaults(http => http.AddStandardResilienceHandler());
        builder.Services.AddOpenTelemetry().WithTracing(t => t.AddSource("TrailRent.Messaging"))
            .WithMetrics(m => m.AddMeter("Orders.Api", "Catalog.Api", "Reviews.Api", "Catalog.Cache", "Polly", "TrailRent.Messaging"));
        builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);
        return builder;
    }

    public static WebApplication MapTrailRentHealthEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready"), ResponseWriter = WriteJsonResponse });
        app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = WriteJsonResponse });
        return app;
    }

    public static Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(new { status = report.Status.ToString(), checks = report.Entries.ToDictionary(x => x.Key, x => new { status = x.Value.Status.ToString(), x.Value.Description }) }));
    }
}
