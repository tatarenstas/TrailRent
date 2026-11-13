using TrailRent.Orders;
using TrailRent.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddTrailRentServiceDefaults();
builder.AddRabbitMQClient("messaging");
builder.Services.AddDbContext<OrdersEventDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("OrdersDb") ?? "Data Source=trailrent-orders.db"));
builder.Services.AddHostedService<EquipmentRenamedConsumer>();
var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.Run();
