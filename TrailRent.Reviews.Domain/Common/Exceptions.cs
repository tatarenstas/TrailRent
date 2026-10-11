namespace TrailRent.Reviews.Domain.Common;
public class DomainException(string message) : Exception(message);
public sealed class NotFoundException(string entity, string id) : DomainException($"{entity} '{id}' was not found.");
public sealed class ConflictException(string message) : DomainException(message);
