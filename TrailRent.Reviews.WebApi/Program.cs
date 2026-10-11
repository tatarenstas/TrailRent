using Serilog;
using Serilog.Context;
using TrailRent.Reviews.Application;
using TrailRent.Reviews.Infrastructure;
using TrailRent.Reviews.Infrastructure.Seeding;
using TrailRent.Reviews.WebApi.Infrastructure;
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, services, config) => config.ReadFrom.Configuration(ctx.Configuration).ReadFrom.Services(services).Enrich.FromLogContext());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddAutoMapper(configuration => configuration.AddMaps(typeof(TrailRent.Reviews.Application.DependencyInjection).Assembly));
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);
var app = builder.Build();
app.Use(async (context, next) => { context.Response.Headers["X-Correlation-Id"] = context.TraceIdentifier; using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier)) await next(); });
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
if (!app.Environment.IsProduction()) { await using var scope = app.Services.CreateAsyncScope(); await scope.ServiceProvider.GetRequiredService<IDataSeeder>().SeedAsync(); }
app.Run();
public partial class Program;
