namespace TrailRent.Orders;

// Maps to ProcessedIntegrationEvents(EventId UNIQUE, ProcessedAt) in the Orders database.
public sealed record ProcessedIntegrationEvent(Guid EventId, DateTimeOffset ProcessedAt);
public sealed class ProcessedEventStore
{
    private readonly HashSet<Guid> _eventIds = [];
    public Task<bool> TryInsertInTransactionAsync(Guid eventId, CancellationToken ct)
    {
        lock (_eventIds) return Task.FromResult(_eventIds.Add(eventId)); // UNIQUE EventId; same transaction as local product update.
    }
    public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;
}

public sealed class LocalEquipmentCatalog
{
    private readonly Dictionary<Guid, string> _names = [];
    public void Rename(Guid equipmentId, string name) => _names[equipmentId] = name; // never changes price recorded on an existing order line.
}
