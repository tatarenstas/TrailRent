using Microsoft.AspNetCore.Mvc;
using TrailRent.Bll.Contracts;

namespace TrailRent.Api.Controllers;

[ApiController]
[Route("api/rentals")]
public sealed class RentalsController(IRentalService rentals) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<RentalDto>>> GetAll(CancellationToken ct) => Ok(await rentals.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RentalDto>> GetById(long id, CancellationToken ct) => Ok(await rentals.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<RentalDto>> Create(CreateRentalRequest input, CancellationToken ct)
    {
        var created = await rentals.CreateAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id:long}/confirm")]
    public async Task<IActionResult> Confirm(long id, CancellationToken ct)
    {
        await rentals.ConfirmAsync(id, ct);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await rentals.DeleteAsync(id, ct);
        return NoContent();
    }
}
