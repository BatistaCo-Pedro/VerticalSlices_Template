namespace Common.Infrastructure.EF.Converters;

public class EntityIdValueConverter<TEntityId> : ValueConverter<TEntityId, Guid>
    where TEntityId : BaseEntityId
{
    public EntityIdValueConverter(ConverterMappingHints mappingHints)
        : base(id => id.Value, value => Create(value), mappingHints) { }

    private static TEntityId Create(Guid id) =>
        (TEntityId)
            Activator.CreateInstance(
                typeof(TEntityId),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                [id],
                culture: null,
                activationAttributes: null
            )!;
}
