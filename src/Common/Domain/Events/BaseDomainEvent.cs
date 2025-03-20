namespace Common.Domain.Events;

/// <summary>
/// The domain event interface.
/// </summary>
public interface IDomainEvent : IEvent, INotification;

/// <summary>
///
/// </summary>
public abstract record BaseDomainEvent : BaseEvent, IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDomainEvent"/> class.
    /// </summary>
    public BaseDomainEvent()
        : base(EventType.Domain) { }

    public BaseDomainEvent(Guid eventId, DateTime occuredOn)
        : base(eventId, occuredOn, EventType.Domain) { }
}
