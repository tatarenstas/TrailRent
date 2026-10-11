using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TrailRent.Catalog.Api.Infrastructure;
using TrailRent.Catalog.Bll.Dtos;
using TrailRent.Catalog.Bll.Services;

namespace TrailRent.Catalog.Api.Controllers;

[ApiController]
[Route("api/equipment")]
public sealed class EquipmentController(IEquipmentService service, IValidator<CreateEquipmentRequest> createValidator, IValidator<EquipmentQuery> queryValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EquipmentListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EquipmentListItemDto>>> GetPage([FromQuery] EquipmentQuery query, CancellationToken cancellationToken)
    {
        var problem = await queryValidator.ValidateAsync(query, cancellationToken);
        if (problem is not null) return BadRequest(problem);
        return Ok(await service.GetPageAsync(query, cancellationToken));
    }

    [HttpGet("rentable")]
    public async Task<ActionResult<IReadOnlyList<EquipmentListItemDto>>> GetRentable(CancellationToken cancellationToken) => Ok(await service.GetRentableAsync(cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EquipmentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipmentDetailsDto>> Get(int id, CancellationToken cancellationToken) => Ok(await service.GetDetailsAsync(id, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(EquipmentDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EquipmentDetailsDto>> Create(CreateEquipmentRequest request, CancellationToken cancellationToken)
    {
        var problem = await createValidator.ValidateAsync(request, cancellationToken);
        if (problem is not null) return BadRequest(problem);
        var created = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { created.Id }, created);
    }

    [HttpPatch("{id:int}/daily-rate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeDailyRate(int id, ChangeDailyRateRequest request, CancellationToken cancellationToken)
    {
        await service.ChangeDailyRateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/remove-stock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveStock(int id, StockChangeRequest request, CancellationToken cancellationToken)
    {
        await service.RemoveStockAsync(id, request, cancellationToken);
        return NoContent();
    }
}
