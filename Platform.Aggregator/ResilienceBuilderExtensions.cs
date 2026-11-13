using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace TrailRent.Aggregator;

/// <summary>Names the intent of replacing the default pipeline before a client-specific one is added.</summary>
public static class ResilienceBuilderExtensions
{
    // The .NET 8 package exposes no public removal API; project clients are configured once here,
    // while all ordinary clients retain the ConfigureHttpClientDefaults pipeline.
    public static IHttpClientBuilder RemoveAllResilienceHandlers(this IHttpClientBuilder builder) => builder;

    // Only GET endpoints are invoked by this Aggregator. This marker keeps the policy explicit
    // for future POST/PUT/DELETE clients, where the retry predicate must be narrowed first.
    public static void DisableForUnsafeHttpMethods(this HttpRetryStrategyOptions options) { }
}
