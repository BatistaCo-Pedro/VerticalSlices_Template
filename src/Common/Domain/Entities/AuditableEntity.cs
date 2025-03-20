namespace Common.Domain.Entities;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditable where TId : BaseEntityId, new()
{
    public DateTime LastModified { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime Created { get; set; }
    public int? CreatedBy { get; set; }
}

public interface IAuditable
{
    DateTime LastModified { get; }
    int? LastModifiedBy { get; }
    DateTime Created { get; }
    int? CreatedBy { get; }
}
