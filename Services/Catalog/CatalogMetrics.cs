using System.Diagnostics.Metrics;

namespace TrailRent.Catalog;

public sealed class CatalogMetrics
{
    private readonly Counter<long> _updated;
    private readonly Histogram<double> _duration;
    public CatalogMetrics(IMeterFactory factory)
    {
        var meter = factory.Create("Catalog.Api");
        _updated = meter.CreateCounter<long>("products.updated", "{product}", "Updated rental equipment");
        _duration = meter.CreateHistogram<double>("catalog.operation.duration", "ms", "Catalog operation duration");
    }
    public void Updated(string result) => _updated.Add(1, new KeyValuePair<string, object?>("result", result));
    public void Duration(string operation, bool success, double ms) => _duration.Record(ms, new("operation", operation), new("status", success ? "success" : "failure"));
}
