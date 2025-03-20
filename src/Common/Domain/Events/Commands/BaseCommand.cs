namespace Common.Domain.Events.Commands;

public interface ICommand<out TResponse> : IRequest<TResponse>;

public record BaseCommand<TResponse> : BaseEvent, ICommand<Result<TResponse>>
{
    public BaseCommand()
        : base(EventType.Command) { }

    public BaseCommand(Guid eventId, DateTime occurredOn)
        : base(eventId, occurredOn, EventType.Command) { }
}

public record BaseCommand : BaseEvent, ICommand<Result>
{
    public BaseCommand()
        : base(EventType.Command) { }

    public BaseCommand(Guid eventId, DateTime occurredOn)
        : base(eventId, occurredOn, EventType.Command) { }
}

