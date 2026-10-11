using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TrailRent.Catalog.Domain;

namespace TrailRent.Catalog.Api.Infrastructure;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            EntityNotFoundException => (StatusCodes.Status404NotFound, "Entity was not found"),
            ValidationException => (StatusCodes.Status400BadRequest, "Domain validation failed"),
            ConflictException or DomainException => (StatusCodes.Status409Conflict, "Business rule conflict"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected server error")
        };
        logger.LogError(exception, "Request {TraceId} failed with HTTP {StatusCode}", context.TraceIdentifier, status);
        var problem = new ProblemDetails { Status = status, Title = title, Detail = status == 500 ? "An unexpected error occurred." : exception.Message, Instance = context.Request.Path };
        problem.Extensions["traceId"] = context.TraceIdentifier;
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
