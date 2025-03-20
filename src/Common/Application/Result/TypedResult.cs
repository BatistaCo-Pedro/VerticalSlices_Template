namespace Common.Application.Result;

/// <summary>
/// Default implementation of <see cref="IResult{TValue}"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value in the result.</typeparam>
/// <remarks>
/// Structs don't return null, instead they return their default value.
/// There might be a need to handle classes and structs differently.
/// </remarks>
[Serializable]
public partial class Result<TValue> : Result, IResult<TValue, Result<TValue>, Error>
{
    /// <inheritdoc />
    [JsonPropertyName("value")]
    public TValue? ValueOrDefault { get; init; }

    /// <inheritdoc />
    [JsonIgnore] // ignore as it throws when value is null
    public TValue Value
    {
        get
        {
            ThrowIfFailed();
            return ValueOrDefault!;
        }
        init
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            ValueOrDefault = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="errors">The errors to set.</param>
    /// <param name="valueOrDefault">The value to set.</param>
    /// <param name="errorId">The ID of the error or null if the result is successful.</param>
    /// <remarks>
    /// Ctor. used for serialization and deserialization.
    /// </remarks>
    [JsonConstructor]
    protected Result(ImmutableList<Error> errors, TValue? valueOrDefault, Guid? errorId)
        : base(errors, errorId)
    {
        ValueOrDefault = valueOrDefault;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    protected Result() { }

    /// <summary>
    /// Initializes a new instance of the<see cref="Result{TValue}"/> class with the specified error.
    /// </summary>
    /// <param name="error">The error to initialize with.</param>
    protected Result(Error error)
        : base(error) { }

    /// <summary>
    /// Initializes a new instance of the<see cref="Result{TValue}"/> class with the specified errors.
    /// </summary>
    /// <param name="errors">The errors to initialize with.</param>
    protected Result(ImmutableList<Error> errors)
        : base(errors) { }

    /// <summary>
    /// Initializes a new instance of the<see cref="Result{TValue}"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value to initialize with.</param>
    protected Result(TValue value)
    {
        Value = value;
    }

    /// <inheritdoc />
    public static Result<TValue> Ok(TValue value) => new(value);

    /// <inheritdoc />
    public new static Result<TValue> Fail() => new(Error.Empty);

    /// <inheritdoc />
    public new static Result<TValue> Fail(string errorMessage) => Fail(new Error(errorMessage));

    /// <inheritdoc />
    public new static Result<TValue> Fail(Exception exception) => new(new Error(exception));

    /// <inheritdoc />
    public new static Result<TValue> Fail(Error error) => new(error);

    /// <inheritdoc />
    public new static Result<TValue> Fail(IEnumerable<Error> errors) =>
        new(errors.ToImmutableList());

    /// <inheritdoc />
    public void Match(Action<TValue> onSuccess, Action<IEnumerable<Error>> onFailure)
    {
        if (IsSuccess)
        {
            onSuccess(Value);
        }
        else
        {
            onFailure(Errors);
        }
    }

    /// <inheritdoc />
    public T Match<T>(Func<TValue, T> onSuccess, Func<IEnumerable<Error>, T> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Errors);

    /// <inheritdoc />
    public Result<TValue> Match(Func<TValue, Result<TValue>> onSuccess) =>
        IsSuccess ? onSuccess(Value) : this;

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <typeparam name="T">The type of the result value being returned.</typeparam>
    /// <returns>A result containing value of type <see cref="T"/>.</returns>
    public Result<T> Match<T>(Func<TValue, Result<T>> onSuccess) =>
        IsSuccess ? onSuccess(Value) : Result<T>.Fail(Errors);

    /// <inheritdoc />
    public Result Match(Func<TValue, Result> onSuccess) =>
        IsSuccess ? onSuccess(Value) : Result.Fail(Errors);

    /// <inheritdoc />
    public Result<TValue> Match(Func<Result<TValue>> onSuccess) => IsSuccess ? onSuccess() : this;

    /// <inheritdoc />
    public async Task<T> MatchAsync<T>(
        Func<TValue, Task<T>> onSuccess,
        Func<IEnumerable<Error>, Task<T>> onFailure
    ) => IsSuccess ? await onSuccess(Value) : await onFailure(Errors);

    /// <inheritdoc />
    public async Task<Result<TValue>> MatchAsync(Func<TValue, Task<Result<TValue>>> onSuccess) =>
        IsSuccess ? await onSuccess(Value) : this;

    /// <inheritdoc />
    public async Task<Result<TValue>> MatchAsync(Func<Task<Result<TValue>>> onSuccess) =>
        IsSuccess ? await onSuccess() : this;

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <typeparam name="T">The type of the result value being returned.</typeparam>
    /// <returns>A result containing value of type <see cref="T"/>.</returns>
    public async Task<Result<T>> MatchAsync<T>(Func<TValue, Task<Result<T>>> onSuccess) =>
        IsSuccess ? await onSuccess(Value) : Result<T>.Fail(Errors);

    /// <summary>
    /// Matches the result and executes the specified actions.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <returns>An instance of type <see cref="Result"/>.</returns>
    public new async Task<Result> MatchAsync(Func<Task<Result>> onSuccess) =>
        IsSuccess ? await onSuccess() : this;

    /// <summary>
    /// Implicitly convert an error to a failed result.
    /// </summary>
    /// <param name="value">The value to convert and include in the result.</param>
    /// <returns>A successful <see cref="Result{TValue}"/> with the value.</returns>
    public static implicit operator Result<TValue>(TValue value)
    {
        if (value is Result<TValue> r)
            return r;

        return Ok(value);
    }

    /// <summary>
    /// Implicitly convert an error to a failed result.
    /// </summary>
    /// <param name="error">The error to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> with the error.</returns>
    public static implicit operator Result<TValue>(Error? error)
    {
        return Fail(error ?? Error.Empty);
    }

    /// <summary>
    /// Implicitly convert a list of errors to a failed result.
    /// </summary>
    /// <param name="errors">The errors to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> with the errors.</returns>
    public static implicit operator Result<TValue>(List<Error> errors)
    {
        return Fail(errors);
    }

    /// <summary>
    /// Implicitly convert a list of errors to a failed result.
    /// </summary>
    /// <param name="errors">The errors to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> with the errors.</returns>
    public static implicit operator Result<TValue>(HashSet<Error> errors)
    {
        return Fail(errors);
    }

    /// <summary>
    /// Implicitly convert a list of errors to a failed result.
    /// </summary>
    /// <param name="errors">The errors to convert and include in the result.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> with the errors.</returns>
    public static implicit operator Result<TValue>(ImmutableList<Error> errors)
    {
        return Fail(errors);
    }

    /// <summary>
    /// Implicitly convert a result to its value.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The value of the result.</returns>
    public static implicit operator TValue(Result<TValue> result)
    {
        return result.Value;
    }

    /// <summary>
    /// Implicitly convert a result to its error list.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The error list of the result.</returns>
    public static implicit operator ImmutableList<Error>(Result<TValue> result)
    {
        return result.Errors;
    }

    /// <summary>
    /// Deconstruct Result.
    /// </summary>
    /// <param name="isSuccess">Bool defining if the result is successful.</param>
    /// <param name="value">The value of the result in case of success or the default of the value.</param>
    /// <param name="errors">The errors from the result - empty in case of success.</param>
    public void Deconstruct(out bool isSuccess, out TValue? value, out ImmutableList<Error> errors)
    {
        isSuccess = IsSuccess;
        value = ValueOrDefault;
        errors = Errors;
    }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this);
}
