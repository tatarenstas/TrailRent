using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TrailRent.Reviews.Domain.Common;
namespace TrailRent.Reviews.WebApi.Infrastructure;
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        { NotFoundException => (404, "Not found"), ValidationException => (400, "Validation failed"), ConflictException => (409, "Conflict"), DomainException => (400, "Domain rule failed"), MongoWriteException { WriteError.Category: ServerErrorCategory.DuplicateKey } => (409, "Duplicate value"), MongoConnectionException => (503, "MongoDB unavailable"), MongoException => (400, "MongoDB operation failed"), _ => (500, "Unexpected error") };
        logger.LogError(exception, "API exception {ExceptionType}; trace {TraceId}; status {StatusCode}", exception.GetType().Name, context.TraceIdentifier, status);
        var details = new ProblemDetails { Status = status, Title = title, Detail = status == 500 ? "An unexpected error occurred." : exception.Message, Instance = context.Request.Path };
        details.Extensions["traceId"] = context.TraceIdentifier;
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(details, ct);
        return true;
    }
}
