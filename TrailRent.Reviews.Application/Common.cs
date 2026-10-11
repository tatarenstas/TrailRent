using MediatR;
namespace TrailRent.Reviews.Application;
public interface ICommand : IRequest { }
public interface ICommand<out T> : IRequest<T> { }
public interface IQuery<out T> : IRequest<T> { }
public sealed record PagedResult<T>(IReadOnlyList<T> Items, long TotalCount, int Page, int PageSize, int TotalPages, bool HasNext);
