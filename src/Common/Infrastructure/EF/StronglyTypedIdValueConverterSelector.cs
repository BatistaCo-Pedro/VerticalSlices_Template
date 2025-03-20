namespace Common.Infrastructure.EF;

public class StronglyTypedIdValueConverterSelector : ValueConverterSelector
{
    private readonly ConcurrentDictionary<string, ValueConverterInfo> _converters = new(
        StringComparer.OrdinalIgnoreCase
    );

    public StronglyTypedIdValueConverterSelector(ValueConverterSelectorDependencies dependencies)
        : base(dependencies) { }

    public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type? providerClrType = null)
    {
        foreach (var converter in base.Select(modelClrType, providerClrType))
        {
            yield return converter;
        }

        var underlyingModelType = UnwrapNullableType(modelClrType);

        var actualModelType = underlyingModelType ?? modelClrType;
        var entityIdType = actualModelType.GetGenericArguments().First();

        var entityIdValueConverterType = typeof(EntityIdValueConverter<>).MakeGenericType(entityIdType);

        yield return _converters.GetOrAdd(
            actualModelType.Name,
            _ =>
            {
                return new ValueConverterInfo(
                    modelClrType: actualModelType,
                    providerClrType: typeof(Guid),
                    factory: info =>
                        (ValueConverter)Activator.CreateInstance(entityIdValueConverterType, info.MappingHints)!
                );
            }
        );
    }

    private static Type? UnwrapNullableType(Type? type)
    {
        if (type is null)
        {
            return null;
        }

        return Nullable.GetUnderlyingType(type) ?? type;
    }
}
