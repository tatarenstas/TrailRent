using MediatR;
using MongoDB.Bson;
using TrailRent.Reviews.Domain.Common;
using TrailRent.Reviews.Domain.Entities;
using TrailRent.Reviews.Domain.Interfaces;
using TrailRent.Reviews.Domain.ValueObjects;
namespace TrailRent.Reviews.Application.Commands;
public sealed class CreateReviewCommandHandler(IReviewRepository reviews) : IRequestHandler<CreateReviewCommand, string>
{
    public async Task<string> Handle(CreateReviewCommand request, CancellationToken ct)
    { if (await reviews.ExistsAsync(request.ProductId, request.AuthorId, ct)) throw new ConflictException("This author has already reviewed this product."); var review = new Review(new ProductReference(request.ProductId, request.ProductName), new Author(request.AuthorId, request.AuthorName), request.Rating, request.Text); await reviews.AddAsync(review, ct); return review.Id.ToString(); }
}
public sealed class UpdateReviewCommandHandler(IReviewRepository reviews) : IRequestHandler<UpdateReviewCommand>
{
    public async Task Handle(UpdateReviewCommand request, CancellationToken ct)
    { var review = await reviews.GetByIdAsync(ObjectId.Parse(request.Id), ct) ?? throw new NotFoundException("Review", request.Id); review.Update(request.Rating, request.Text); await reviews.ReplaceAsync(review, request.ExpectedVersion, ct); }
}
public sealed class DeleteReviewCommandHandler(IReviewRepository reviews) : IRequestHandler<DeleteReviewCommand>
{
    public Task Handle(DeleteReviewCommand request, CancellationToken ct) => reviews.DeleteAsync(ObjectId.Parse(request.Id), request.ExpectedVersion, ct);
}
public sealed class AddCommentCommandHandler(IReviewRepository reviews) : IRequestHandler<AddCommentCommand>
{
    public async Task Handle(AddCommentCommand request, CancellationToken ct)
    { var review = await reviews.GetByIdAsync(ObjectId.Parse(request.Id), ct) ?? throw new NotFoundException("Review", request.Id); review.AddComment(new Author(request.AuthorId, request.AuthorName), request.Text); await reviews.ReplaceAsync(review, request.ExpectedVersion, ct); }
}
