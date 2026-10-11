using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrailRent.Reviews.Application;
using TrailRent.Reviews.Application.Commands;
using TrailRent.Reviews.Application.Dtos;
using TrailRent.Reviews.Application.Queries;
namespace TrailRent.Reviews.WebApi.Controllers;
[ApiController]
[Route("api/reviews")]
public sealed class ReviewsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetList([FromQuery] long? productId, [FromQuery] string? text, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) => Ok(await sender.Send(new GetReviewsListQuery(productId, text, page, pageSize), ct));
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReviewDto>> GetById(string id, CancellationToken ct)
    { var review = await sender.Send(new GetReviewByIdQuery(id), ct); Response.Headers.ETag = $"\"{review.Version}\""; return Ok(review); }
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<ActionResult<string>> Create(CreateReviewCommand command, CancellationToken ct)
    { var id = await sender.Send(command, ct); return CreatedAtAction(nameof(GetById), new { id }, id); }
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(string id, UpdateReviewBody body, CancellationToken ct)
    { await sender.Send(new UpdateReviewCommand(id, ExpectedVersion(body.ExpectedVersion), body.Rating, body.Text), ct); return NoContent(); }
    [HttpPost("{id}/comments")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddComment(string id, AddCommentBody body, CancellationToken ct)
    { await sender.Send(new AddCommentCommand(id, ExpectedVersion(body.ExpectedVersion), body.AuthorId, body.AuthorName, body.Text), ct); return NoContent(); }
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(string id, [FromQuery] int expectedVersion, CancellationToken ct)
    { await sender.Send(new DeleteReviewCommand(id, ExpectedVersion(expectedVersion)), ct); return NoContent(); }
    private int ExpectedVersion(int fallback) => int.TryParse(Request.Headers.IfMatch.ToString().Trim('"'), out var version) ? version : fallback;
}
public sealed record UpdateReviewBody(int ExpectedVersion, int Rating, string Text);
public sealed record AddCommentBody(int ExpectedVersion, string AuthorId, string AuthorName, string Text);
