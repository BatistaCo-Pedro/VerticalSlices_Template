namespace Statistics.Domain.Entities;

public record SomeEntityId : BaseEntityId
{
    public SomeEntityId() { }

    protected SomeEntityId(Guid value)
        : base(value) { }
}

public class SomeEntity : AggregateRoot<SomeEntityId>
{
    private readonly List<string> _someStrings = [];

    public SomeEntity(string someString)
    {
        SomeString = someString;
    }

    public string SomeString { get; private set; }

    public IReadOnlyList<string> SomeStrings
    {
        get => _someStrings;
        init => _someStrings = value.ToList();
    }
}
