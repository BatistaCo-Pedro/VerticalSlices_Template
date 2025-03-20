namespace Common.Domain.Events;

public abstract record BaseEvent(Guid EventId, DateTime OccurredOn, EventType EventType) : IEvent
{
    protected BaseEvent(EventType eventType)
        : this(Guid.NewGuid(), DateTime.UtcNow, eventType) { }
}
