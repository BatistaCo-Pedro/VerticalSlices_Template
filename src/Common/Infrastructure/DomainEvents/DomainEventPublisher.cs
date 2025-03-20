namespace Common.Infrastructure.DomainEvents;

public class DomainEventPublisher(IPublisher publisher) : IDomainEventPublisher
{
    public async Task PublishAsync(
        IReadOnlyCollection<IEntity> entitiesWithDomainEvents,
        CancellationToken cancellationToken = default
    )
    {
        if (entitiesWithDomainEvents.Count == 0)
            return;

        foreach (var entity in entitiesWithDomainEvents)
        {
            var domainEvents = entity.DomainEvents.ToArray();
            
            entity.ClearDomainEvents();
            
            foreach (var domainEvent in domainEvents)
            {
                await publisher.Publish(domainEvent, cancellationToken);
                
                Log.Debug(
                    "Dispatched domain event {DomainEventName} with payload {DomainEventContent}",
                    domainEvent.EventType.ToString(),
                    domainEvent
                );
            }
        }
    }
}
