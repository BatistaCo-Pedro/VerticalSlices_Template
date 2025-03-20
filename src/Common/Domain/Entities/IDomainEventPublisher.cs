namespace Common.Domain.Entities;

public interface IDomainEventPublisher
{
    Task PublishAsync(IReadOnlyCollection<IEntity> entitiesWithDomainEvents, CancellationToken cancellationToken = default);
}
