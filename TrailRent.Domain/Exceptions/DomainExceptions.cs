namespace TrailRent.Domain.Exceptions;

public sealed class NotFoundException(string message) : Exception(message);
public sealed class BusinessConflictException(string message) : Exception(message);
public sealed class DomainValidationException(string message) : Exception(message);
