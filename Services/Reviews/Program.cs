using TrailRent.Reviews;
using TrailRent.ServiceDefaults;
var builder = WebApplication.CreateBuilder(args);
builder.AddTrailRentServiceDefaults();
builder.AddRabbitMQClient("messaging");
builder.Services.AddSingleton<ReviewsEventStore>(); builder.Services.AddSingleton<ReviewsProjection>(); builder.Services.AddHostedService<EquipmentRenamedConsumer>();
var app = builder.Build(); app.MapGet("/health", () => Results.Ok(new { status = "Healthy" })); app.Run();
