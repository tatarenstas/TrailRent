using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Platform.ServiceDefaults;

public static class Extensions
{
    public const string CorrelationHeader = "X-Correlation-Id";
    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddSerilog((_, log) => log.MinimumLevel.Is(builder.Environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information).MinimumLevel.Override("Microsoft", LogEventLevel.Warning).Enrich.FromLogContext().Enrich.WithProperty("ServiceName", builder.Environment.ApplicationName).Enrich.WithEnvironmentName().Enrich.WithMachineName().Enrich.WithThreadId().WriteTo.Console(new CompactJsonFormatter()), writeToProviders: true);
        var telemetry = builder.Services.AddOpenTelemetry().WithMetrics(m => m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddRuntimeInstrumentation()).WithTracing(t => { t.AddSource(builder.Environment.ApplicationName).AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddSqlClientInstrumentation(); if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"])) t.AddOtlpExporter(); });
        builder.Services.AddHealthChecks();
        builder.Services.AddServiceDiscovery();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<CorrelationIdHandler>();
        builder.Services.ConfigureHttpClientDefaults(http => { http.AddStandardResilienceHandler(); http.AddServiceDiscovery(); http.AddHttpMessageHandler<CorrelationIdHandler>(); });
        return builder;
    }
    public static WebApplication UseCorrelationId(this WebApplication app)
    {
        app.Use(async (context, next) => { var id = context.Request.Headers[CorrelationHeader].FirstOrDefault() ?? Guid.NewGuid().ToString(); context.Request.Headers[CorrelationHeader] = id; context.Response.Headers[CorrelationHeader] = id; using (LogContext.PushProperty("CorrelationId", id)) { app.Logger.LogInformation("Request started {Method} {Path}", context.Request.Method, context.Request.Path); await next(); app.Logger.LogInformation("Request finished {StatusCode}", context.Response.StatusCode); } });
        return app;
    }
    public static WebApplication MapDefaultEndpoints(this WebApplication app) { app.MapHealthChecks("/health"); app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = _ => false }); return app; }
}

public sealed class CorrelationIdHandler(IHttpContextAccessor accessor) : DelegatingHandler
{ protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) { var id = accessor.HttpContext?.Request.Headers[Extensions.CorrelationHeader].FirstOrDefault(); if (!string.IsNullOrEmpty(id)) request.Headers.TryAddWithoutValidation(Extensions.CorrelationHeader, id); return base.SendAsync(request, ct); } }
