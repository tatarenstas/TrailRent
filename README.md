# TrailRent gRPC & cache platform — Lab 06

Independent implementation of Lab 06: gRPC contracts for Orders, Catalog and Reviews; parallel gRPC aggregation; L1 `IMemoryCache` + L2 `IDistributedCache`; Redis resource in Aspire AppHost.

## Key flow

`Aggregator → gRPC clients → Orders/Catalog/Reviews`. Catalog loads `L1 → L2 → source`; each successful miss repopulates both levels. Product writes invalidate both keys. The aggregate uses `Task.WhenAll`, turns unavailable gRPC services into warnings, and only caches complete responses for 30 seconds.

## Run

```bash
dotnet build TrailRent.GrpcPlatform.sln
dotnet run --project Platform.AppHost
```

In Aspire, reference resource `redis` from Catalog and Aggregator (`WithReference(redis).WaitFor(redis)`). The code keeps `AddDistributedMemoryCache` as a local fallback, so a Redis outage does not stop the service. For production/Aspire replace it with `builder.AddRedisDistributedCache("redis")` from `Aspire.StackExchange.Redis.DistributedCaching`.

See [docs/lab-06/checklist.md](docs/lab-06/checklist.md) for Dashboard proof points.
