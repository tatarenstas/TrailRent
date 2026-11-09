using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Platform.Aggregator.Clients;
namespace Platform.Aggregator.Controllers;
[ApiController, Route("api/aggregator/orders")]
public sealed class OrdersAggregatorController(OrdersClient orders, CatalogClient catalog, ReviewsClient reviews, ILogger<OrdersAggregatorController> logger) : ControllerBase
{
    [HttpGet("{id:long}/full")]
    public async Task<ActionResult<FullOrderDto>> GetFull(long id, CancellationToken ct)
    {
        var order = await orders.GetAsync(id, ct); if (order is null) return NotFound(); var warnings = new List<string>(); var ids = order.Items.Select(x => x.ProductId).Distinct().ToArray();
        var productsTask = SafeAsync(() => catalog.GetAsync(ids, ct), "catalog-api", warnings); var reviewTasks = ids.Select(x => SafeAsync(() => reviews.GetAsync(x, ct), "reviews-api", warnings)).ToArray(); await Task.WhenAll(productsTask, Task.WhenAll(reviewTasks));
        return Ok(new FullOrderDto(order, await productsTask ?? [], (await Task.WhenAll(reviewTasks)).Where(x => x is not null).SelectMany(x => x!).ToList(), warnings));
    }
    private async Task<T?> SafeAsync<T>(Func<Task<T?>> call, string service, List<string> warnings) { var watch = Stopwatch.StartNew(); try { var result = await call(); logger.LogInformation("{Service} responded in {ElapsedMs}ms", service, watch.ElapsedMilliseconds); return result; } catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException) { logger.LogWarning(ex, "{Service} is unavailable", service); lock (warnings) warnings.Add($"{service} is unavailable"); return default; } }
}
