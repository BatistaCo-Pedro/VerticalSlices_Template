namespace Common.Domain.Entities;

public abstract class AggregateRoot<TId> : AuditableEntity<TId> where TId : BaseEntityId, new();