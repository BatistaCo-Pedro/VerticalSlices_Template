namespace Common.Domain.Events;

public interface IDomainEventHandler<in TDomainEvent> : IEventHandler, INotificationHandler<TDomainEvent>
where TDomainEvent : IDomainEvent;


