namespace Common.Domain.Events.Queries;

public interface IQuery<out TResponse> : IRequest<TResponse>;

public record BaseQuery<TResponse> : BaseEvent, IQuery<Result<TResponse>>
{
    public BaseQuery()
        : base(EventType.Query) { }

    public BaseQuery(Guid eventId, DateTime occurredOn)
        : base(eventId, occurredOn, EventType.Query) { }
}
