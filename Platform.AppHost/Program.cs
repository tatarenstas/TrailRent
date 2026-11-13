var builder = DistributedApplication.CreateBuilder(args);
var redis = builder.AddRedis("redis").WithDataVolume();
var catalog = builder.AddProject<Projects.Services_Catalog>("catalog-api").WithReference(redis).WaitFor(redis).WithHttpHealthCheck("/health");
var orders = builder.AddProject<Projects.Services_Orders>("orders-api").WithReference(redis).WaitFor(redis).WithHttpHealthCheck("/health");
var reviews = builder.AddProject<Projects.Services_Reviews>("reviews-api").WithHttpHealthCheck("/health");
builder.AddProject<Projects.Platform_Aggregator>("aggregator").WithReference(catalog).WithReference(orders).WithReference(reviews).WaitFor(catalog).WaitFor(orders).WaitFor(reviews).WithHttpHealthCheck("/health");
builder.Build().Run();
