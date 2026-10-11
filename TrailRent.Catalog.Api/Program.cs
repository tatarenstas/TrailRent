using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using TrailRent.Catalog.Api.Infrastructure;
using TrailRent.Catalog.Bll.Mapping;
using TrailRent.Catalog.Bll.Services;
using TrailRent.Catalog.Bll.Validators;
using TrailRent.Catalog.Dal;
using TrailRent.Catalog.Dal.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, logger) => logger.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).Enrich.FromLogContext());
builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddAutoMapper(configuration => configuration.AddProfile<CatalogProfile>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateEquipmentValidator>();
builder.Services.AddDbContext<CatalogDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Catalog")));
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IEquipmentCategoryRepository, EquipmentCategoryRepository>();
builder.Services.AddScoped<IEquipmentFeatureRepository, EquipmentFeatureRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Correlation-Id"] = context.TraceIdentifier;
    using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier)) await next();
});
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();

if (!app.Environment.IsProduction())
{
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await context.Database.MigrateAsync();
    await CatalogSeeder.SeedAsync(context);
}
app.Run();

public partial class Program;
