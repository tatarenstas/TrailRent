namespace TrailRent.Reviews.Application.Commands;
public sealed record CreateReviewCommand(long ProductId, string ProductName, string AuthorId, string AuthorName, int Rating, string Text) : ICommand<string>;
public sealed record UpdateReviewCommand(string Id, int ExpectedVersion, int Rating, string Text) : ICommand;
public sealed record DeleteReviewCommand(string Id, int ExpectedVersion) : ICommand;
public sealed record AddCommentCommand(string Id, int ExpectedVersion, string AuthorId, string AuthorName, string Text) : ICommand;
