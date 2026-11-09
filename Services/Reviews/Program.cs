using Platform.ServiceDefaults;
var builder = WebApplication.CreateBuilder(args); builder.AddServiceDefaults(); var app = builder.Build(); app.UseCorrelationId(); app.MapGet("/api/reviews", (long? productId) => Results.Ok(new[] { new ReviewDto("r1", 101, "Олена", 5, "Витримав дощ"), new ReviewDto("r2", 205, "Марта", 5, "Дуже легкі") }.Where(r => !productId.HasValue || r.ProductId == productId))); app.MapDefaultEndpoints(); app.Run();
public sealed record ReviewDto(string Id, long ProductId, string Author, int Rating, string Text); public partial class Program;
