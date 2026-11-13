using TrailRent.Orders;
using TrailRent.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddTrailRentServiceDefaults();
builder.AddRabbitMQClient("messaging");
builder.Services.AddSingleton<ProcessedEventStore>();
builder.Services.AddSingleton<LocalEquipmentCatalog>();
builder.Services.AddHostedService<EquipmentRenamedConsumer>();
var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.Run();
