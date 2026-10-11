using FluentValidation;
using MongoDB.Bson;
namespace TrailRent.Reviews.Application.Queries;
public sealed class GetReviewByIdQueryValidator : AbstractValidator<GetReviewByIdQuery> { public GetReviewByIdQueryValidator() => RuleFor(x => x.Id).Must(x => ObjectId.TryParse(x, out _)).WithMessage("Id must be a valid ObjectId."); }
public sealed class GetReviewsListQueryValidator : AbstractValidator<GetReviewsListQuery> { public GetReviewsListQueryValidator() { RuleFor(x => x.Page).GreaterThan(0); RuleFor(x => x.PageSize).InclusiveBetween(1, 100); } }
