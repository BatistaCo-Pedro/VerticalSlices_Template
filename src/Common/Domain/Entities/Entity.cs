namespace Common.Domain.Entities;

public abstract class Entity<TId> : IEntity<TId> where TId : BaseEntityId, new()
{
    private readonly ConcurrentQueue<IDomainEvent> _domainEvents = new();

    public TId Id { get; init; } = new();

    public ImmutableQueue<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Enqueue(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}

public interface IEntity<out TId> : IEntity where TId : BaseEntityId, new()
{
    /// <summary>
    /// Gets the entity identifier.
    /// </summary>
    TId Id { get; }
}

public interface IEntity
{
    /// <summary>
    /// Gets the domain events.
    /// </summary>
    [NotMapped]
    ImmutableQueue<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Raise a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    void RaiseDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Remove all domain events.
    /// </summary>
    void ClearDomainEvents();
}