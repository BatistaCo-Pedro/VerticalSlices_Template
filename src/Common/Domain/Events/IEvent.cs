namespace Common.Domain.Events;

/// <summary>
/// The event interface.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Gets the event identifier.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the date the <see cref="IEvent"/> occurred on.
    /// </summary>
    DateTime OccurredOn { get; }
    
    /// <summary>
    /// Gets the event type.
    /// </summary>
    EventType EventType { get; }
}
