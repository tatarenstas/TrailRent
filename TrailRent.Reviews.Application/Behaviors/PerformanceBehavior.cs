using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
namespace TrailRent.Reviews.Application.Behaviors;
public sealed class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    { var watch = Stopwatch.StartNew(); var result = await next(); if (watch.ElapsedMilliseconds > 500) logger.LogWarning("Slow request {RequestName}: {ElapsedMs}ms", typeof(TRequest).Name, watch.ElapsedMilliseconds); return result; }
}
