# Metric contract

`orders.created` and `orders.confirmed` are monotonic counters tagged only with low-cardinality `order.status` and `payment.method`. `orders.operation.duration` and `catalog.operation.duration` record both success and failure, in milliseconds, tagged with `operation` and `status`. `orders.awaiting_validation` is an `ObservableGauge` snapshot, not a counter.

No user id, order id, timestamp or request id is used as a metric tag. These would create an unbounded number of time series. During an incident, filter the Dashboard by service, operation, status and Polly `event.name`; correlate the matching trace using its TraceId.
