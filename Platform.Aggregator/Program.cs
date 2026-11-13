using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using TrailRent.Aggregator;
using TrailRent.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddTrailRentServiceDefaults();
builder.Services.AddHttpClient<CatalogClient>(c => c.BaseAddress = new Uri("https://catalog-api")).RemoveAllResilienceHandlers().AddStandardResilienceHandler(o =>
{
    o.Retry.MaxRetryAttempts = 5; o.Retry.Delay = TimeSpan.FromSeconds(1); o.Retry.BackoffType = DelayBackoffType.Exponential; o.Retry.UseJitter = true; o.Retry.DisableForUnsafeHttpMethods();
    o.CircuitBreaker.FailureRatio = .3; o.CircuitBreaker.MinimumThroughput = 10; o.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30); o.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
    o.CircuitBreaker.OnOpened = args => { Console.Error.WriteLine($"OnCircuitOpened catalog for {args.BreakDuration}"); return ValueTask.CompletedTask; };
    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3); o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(25);
});
// Reviews are intentionally less critical: fail fast and let the aggregator return a partial response.
builder.Services.AddHttpClient<ReviewsClient>(c => c.BaseAddress = new Uri("https://reviews-api")).RemoveAllResilienceHandlers().AddStandardResilienceHandler(o =>
{
    o.Retry.MaxRetryAttempts = 1; o.Retry.DisableForUnsafeHttpMethods(); o.CircuitBreaker.FailureRatio = .7; o.CircuitBreaker.MinimumThroughput = 5; o.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(15); o.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(5); o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(2); o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(5);
});
var retry = new RetryPolicy { MaxAttempts = 4, InitialBackoff = TimeSpan.FromSeconds(.5), MaxBackoff = TimeSpan.FromSeconds(4), BackoffMultiplier = 2 };
retry.RetryableStatusCodes.Add(StatusCode.Unavailable); retry.RetryableStatusCodes.Add(StatusCode.ResourceExhausted);
var config = new ServiceConfig { MethodConfigs = { new MethodConfig { Names = { MethodName.Default }, RetryPolicy = retry } } };
builder.Services.AddGrpcClient<Health.HealthClient>(o => o.Address = new Uri("https://catalog-api")).ConfigureChannel(channel => channel.ServiceConfig = config);
builder.Services.AddHealthChecks().AddCheck<CatalogGrpcHealthCheck>("catalog-api", tags: ["grpc", "downstream", "ready"]);
var app = builder.Build();
app.MapGet("/api/aggregator/orders/{id:guid}/full", (Guid id) => Results.Ok(new { id, partial = false }));
app.MapTrailRentHealthEndpoints();
app.Run();
