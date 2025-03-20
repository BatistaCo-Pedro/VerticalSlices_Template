namespace Common.Application.Validation;

public class ValidationError(ValidationFailure validationFailure)
    : Error(validationFailure.ErrorMessage, validationFailure.GetMetaData());
