namespace Common.Application.Result;

/// <summary>
/// Exception thrown when a result is failed.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ResultFailedException{TError}"/> class.
/// </remarks>
/// <param name="result">The result to get the errors from.</param>
public class ResultFailedException<TError>(IResult<TError> result) : Exception(result.ToString())
    where TError : IError;
