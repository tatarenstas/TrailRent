using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;

namespace TrailRent.ServiceDefaults;

public static class TelemetryExtensions
{
    public static TBuilder AddTrailRentServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource("TrailRent.Messaging"));
        return builder;
    }
}
