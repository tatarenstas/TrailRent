# Lab 06 checklist

- Three stable protobuf contracts define service and message fields.
- Catalog exposes gRPC and uses a reusable two-level cache with L1 size/TTL policy, L2 serialization, invalidation and structured logs.
- Cache counters `cache.hits` and `cache.misses` use `IMeterFactory` under `Catalog.Cache`.
- Aggregator uses generated typed gRPC clients, `Task.WhenAll`, cancellation and `RpcException` partial-failure handling; only fully successful aggregates are cached.
- AppHost declares persistent Redis. Capture cache hit metrics, gRPC client/server spans, and a repeated item request after Redis is available.
