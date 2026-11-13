using System.Diagnostics.Metrics;

namespace TrailRent.Orders;

public sealed class OrdersSnapshot { public int AwaitingValidation { get; private set; } public void IncrementAwaiting() => AwaitingValidation++; }

public sealed class OrderMetrics
{
    private readonly Counter<long> _created;
    private readonly Counter<long> _confirmed;
    private readonly Histogram<double> _duration;
    public OrderMetrics(IMeterFactory factory, OrdersSnapshot snapshot)
    {
        var meter = factory.Create("Orders.Api");
        _created = meter.CreateCounter<long>("orders.created", "{order}", "Successfully created rental orders");
        _confirmed = meter.CreateCounter<long>("orders.confirmed", "{order}", "Confirmed rental orders");
        _duration = meter.CreateHistogram<double>("orders.operation.duration", "ms", "Duration of order operations");
        meter.CreateObservableGauge("orders.awaiting_validation", () => snapshot.AwaitingValidation, "{order}", "Orders waiting for stock validation");
    }
    public void Created(string status, string payment) => _created.Add(1, new("order.status", status), new("payment.method", payment));
    public void Confirmed(string status) => _confirmed.Add(1, new KeyValuePair<string, object?>("order.status", status));
    public void Duration(string operation, bool success, double ms) => _duration.Record(ms, new("operation", operation), new("status", success ? "success" : "failure"));
}
