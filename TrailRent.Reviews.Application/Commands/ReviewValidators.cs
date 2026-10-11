using FluentValidation;
using MongoDB.Bson;
using TrailRent.Reviews.Domain.Interfaces;
namespace TrailRent.Reviews.Application.Commands;
public sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator(IReviewRepository reviews)
    { RuleFor(x => x.ProductId).GreaterThan(0); RuleFor(x => x.ProductName).NotEmpty().MaximumLength(160); RuleFor(x => x.AuthorId).NotEmpty().MaximumLength(64); RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(100); RuleFor(x => x.Rating).InclusiveBetween(1, 5); RuleFor(x => x.Text).NotEmpty().MaximumLength(2000); RuleFor(x => x).MustAsync(async (x, ct) => !await reviews.ExistsAsync(x.ProductId, x.AuthorId, ct)).WithMessage("Duplicate review."); }
}
public sealed class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand> { public UpdateReviewCommandValidator() { RuleFor(x => x.Id).Must(x => ObjectId.TryParse(x, out _)); RuleFor(x => x.ExpectedVersion).GreaterThanOrEqualTo(0); RuleFor(x => x.Rating).InclusiveBetween(1, 5); RuleFor(x => x.Text).NotEmpty().MaximumLength(2000); } }
public sealed class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand> { public DeleteReviewCommandValidator() { RuleFor(x => x.Id).Must(x => ObjectId.TryParse(x, out _)); RuleFor(x => x.ExpectedVersion).GreaterThanOrEqualTo(0); } }
public sealed class AddCommentCommandValidator : AbstractValidator<AddCommentCommand> { public AddCommentCommandValidator() { RuleFor(x => x.Id).Must(x => ObjectId.TryParse(x, out _)); RuleFor(x => x.Text).NotEmpty().MaximumLength(1000); RuleFor(x => x.AuthorId).NotEmpty(); } }
