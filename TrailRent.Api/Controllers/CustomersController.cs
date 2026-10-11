using Microsoft.AspNetCore.Mvc;
using TrailRent.Bll.Contracts;

namespace TrailRent.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(ICustomerService customers) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll(CancellationToken ct) => Ok(await customers.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CustomerDto>> GetById(long id, CancellationToken ct) => Ok(await customers.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(UpsertCustomerRequest input, CancellationToken ct)
    {
        var created = await customers.CreateAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpsertCustomerRequest input, CancellationToken ct)
    {
        await customers.UpdateAsync(id, input, ct);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await customers.DeleteAsync(id, ct);
        return NoContent();
    }
}
