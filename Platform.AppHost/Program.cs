var builder=DistributedApplication.CreateBuilder(args);
var redis=builder.AddRedis("redis").WithDataVolume();
var catalog=builder.AddProject<Projects.Services_Catalog>("catalog-api").WithReference(redis).WaitFor(redis);
var orders=builder.AddProject<Projects.Services_Orders>("orders-api").WithReference(redis).WaitFor(redis);
var reviews=builder.AddProject<Projects.Services_Reviews>("reviews-api").WithReference(redis).WaitFor(redis);
builder.AddProject<Projects.Platform_Aggregator>("aggregator").WithReference(redis).WaitFor(redis).WithReference(catalog).WithReference(orders).WithReference(reviews);
builder.Build().Run();
