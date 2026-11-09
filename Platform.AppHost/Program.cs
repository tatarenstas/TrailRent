var builder=DistributedApplication.CreateBuilder(args);
var redis=builder.AddRedis("redis").WithDataVolume();
// In the full platform these references point to Orders, Catalog, Reviews and Aggregator.
// They receive ConnectionStrings:redis and wait for the shared Redis container.
builder.Build().Run();
