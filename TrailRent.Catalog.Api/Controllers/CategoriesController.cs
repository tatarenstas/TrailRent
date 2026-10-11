using Microsoft.AspNetCore.Mvc;
using TrailRent.Catalog.Bll.Dtos;
using TrailRent.Catalog.Bll.Services;

namespace TrailRent.Catalog.Api.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(ICategoryService service) : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryDetailsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoryDetailsDto>> Get(int id, CancellationToken cancellationToken) => Ok(await service.GetWithEquipmentAsync(id, cancellationToken));

    [HttpGet("feature/{featureName}")]
    public async Task<ActionResult<IReadOnlyList<EquipmentListItemDto>>> GetByFeature(string featureName, [FromQuery] DateTime? since, CancellationToken cancellationToken) =>
        Ok(await service.GetFeatureEquipmentAsync(featureName, since ?? DateTime.UtcNow.AddDays(-30), cancellationToken));
}
