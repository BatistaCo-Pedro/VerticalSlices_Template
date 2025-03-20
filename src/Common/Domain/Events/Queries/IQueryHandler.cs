namespace Common.Domain.Events.Queries;

public interface IQueryHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
where TCommand : IQuery<TResponse>;