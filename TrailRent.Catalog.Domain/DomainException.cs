namespace TrailRent.Catalog.Domain;

public class DomainException(string message) : Exception(message);
public sealed class EntityNotFoundException(string entity, int id) : DomainException($"{entity} with id {id} was not found.");
public sealed class ConflictException(string message) : DomainException(message);
public sealed class ValidationException(string message) : DomainException(message);
