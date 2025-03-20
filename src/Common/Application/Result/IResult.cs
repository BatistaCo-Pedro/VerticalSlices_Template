namespace Common.Application.Result;

/// <summary>
/// Marker interface for results.
/// </summary>
/// <typeparam name="TError">The type of the error.</typeparam>
public interface IResult<TError>
    where TError : IError
{
    /// <summary>
    /// Collection of errors. Empty if the result was successful.
    /// </summary>
    public ImmutableList<TError> Errors { get; }

    /// <summary>
    /// Error ID. Null if the result was successful.
    /// </summary>
    public Guid? ErrorId { get; }

    /// <summary>
    /// Determines whether the result is a success.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Determines whether the result is a failure.
    /// </summary>
    public bool IsFailure { get; }
}

/// <summary>
/// Interface for a typed result.
/// </summary>
/// <typeparam name="TError">The type of the error.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IResult<TResult, TError> : IResult<TError>
    where TError : class, IError
    where TResult : IResult<TResult, TError>
{
    /// <summary>
    /// Creates a faulty result with the specified error message.
    /// </summary>
    /// <returns>A result of type <see cref="TResult"/>.</returns>
    static abstract TResult Fail();

    /// <summary>
    /// Creates a faulty result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A result of type <see cref="TResult"/>.</returns>
    static abstract TResult Fail(string errorMessage);

    /// <summary>
    /// Creates a faulty result with the specified error.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>A result of type <see cref="TResult"/>.</returns>
    static abstract TResult Fail(TError error);

    /// <summary>
    /// Creates a faulty result with the specified errors.
    /// </summary>
    /// <param name="errors">The errors.</param>
    /// <returns>A result of type <see cref="TResult"/>.</returns>
    static abstract TResult Fail(IEnumerable<TError> errors);

    /// <summary>
    /// Creates a faulty result with the specified exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>A result of type <see cref="TResult"/>.</returns>
    static abstract TResult Fail(Exception exception);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    /// <typeparam name="T">The new type being returned.</typeparam>
    /// <returns>An instance of type <see cref="T"/>.</returns>
    T Match<T>(Func<T> onSuccess, Func<IEnumerable<TError>, T> onFailure);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <returns>An instance of type <see cref="TResult"/>.</returns>
    TResult Match(Func<TResult> onSuccess);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    void Match(Action onSuccess, Action<IEnumerable<TError>> onFailure);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    /// <typeparam name="T">The new type being returned.</typeparam>
    /// <returns>An instance of type <see cref="T"/>.</returns>
    Task<T> MatchAsync<T>(Func<Task<T>> onSuccess, Func<IEnumerable<Error>, Task<T>> onFailure);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <returns>An awaitable instance of type <see cref="TResult"/>.</returns>
    Task<TResult> MatchAsync(Func<Task<TResult>> onSuccess);

    /// <summary>
    /// Throws an exception if the result is a failure.
    /// </summary>
    void ThrowIfFailed();
}

/// <summary>
/// Interface for a result, which can be Successful.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IOkResult<out TResult>
{
    /// <summary>
    /// Creates a success result.
    /// </summary>
    /// <returns>A successful <see cref="TResult"/>.</returns>
    static abstract TResult Ok();
}

/// <summary>
/// Interface for a typed result.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IResult<TValue, TResult, TError> : IResult<TResult, TError>
    where TError : class, IError
    where TResult : IResult<TValue, TResult, TError>
{
    /// <summary>
    /// The value.
    /// Throws if result does not contain a value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// The value or default.
    /// </summary>
    public TValue? ValueOrDefault { get; }

    /// <summary>
    /// Creates a success result with the specified value.
    /// </summary>
    /// <param name="value">The value to include in the result.</param>
    /// <returns>A new instance of <see cref="TResult"/> representing a success result with the specified value.</returns>
    public static abstract TResult Ok(TValue value);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    void Match(Action<TValue> onSuccess, Action<IEnumerable<Error>> onFailure);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    /// <typeparam name="T">The new type being returned.</typeparam>
    /// <returns>An instance of type <see cref="T"/>.</returns>
    T Match<T>(Func<TValue, T> onSuccess, Func<IEnumerable<TError>, T> onFailure);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <returns>An instance of type <see cref="Result"/>.</returns>
    Result Match(Func<TValue, Result> onSuccess);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <returns>An instance of type <see cref="TResult"/>.</returns>
    TResult Match(Func<TValue, TResult> onSuccess);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    /// <typeparam name="T">The type of the object being returned.</typeparam>
    /// <returns>An instance of type <see cref="T"/>.</returns>
    Task<T> MatchAsync<T>(
        Func<TValue, Task<T>> onSuccess,
        Func<IEnumerable<Error>, Task<T>> onFailure
    );

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <returns>An instance of type <see cref="TResult"/>.</returns>
    Task<TResult> MatchAsync(Func<TValue, Task<TResult>> onSuccess);
}
