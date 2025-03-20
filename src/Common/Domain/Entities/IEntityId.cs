namespace Common.Domain.Entities;

public abstract record BaseEntityId
{
    public Guid Value { get; init; } = Ulid.NewUlid().ToGuid();

    protected BaseEntityId() { }
    
    protected BaseEntityId(Guid value)
    {
        Value = value;
    }
}