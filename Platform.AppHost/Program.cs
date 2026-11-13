var builder = DistributedApplication.CreateBuilder(args);
var messaging = builder.AddRabbitMQ("messaging").WithDataVolume().WithManagementPlugin();
var catalog = builder.AddProject<Projects.Services_Catalog>("catalog-api").WithReference(messaging).WaitFor(messaging).WithHttpHealthCheck("/health");
var orders = builder.AddProject<Projects.Services_Orders>("orders-api").WithReference(messaging).WaitFor(messaging).WithHttpHealthCheck("/health");
builder.AddProject<Projects.Services_Reviews>("reviews-api").WithReference(messaging).WaitFor(messaging).WithHttpHealthCheck("/health");
builder.Build().Run();
