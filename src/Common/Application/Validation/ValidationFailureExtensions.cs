namespace Common.Application.Validation;

public static class ValidationFailureExtensions
{
    public static ImmutableDictionary<string, string> GetMetaData(this ValidationFailure validationFailure)
    {
        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
               {
                   { nameof(validationFailure.PropertyName), validationFailure.PropertyName },
                   { nameof(validationFailure.ErrorCode), validationFailure.ErrorCode },
                   { nameof(validationFailure.Severity), validationFailure.Severity.ToString() },
                   { nameof(validationFailure.AttemptedValue), JsonSerializer.Serialize(validationFailure.AttemptedValue) },
               }.ToImmutableDictionary();
    }
}
