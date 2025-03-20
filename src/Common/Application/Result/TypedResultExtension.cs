namespace Common.Application.Result;

/// <summary>
/// Partial class of <see cref="Result{TValue}"/> for extensions.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public partial class Result<TValue>
{
    /// <summary>
    /// Logs the result.
    /// </summary>
    /// <param name="messageTemplate">The template of the message to log.</param>
    /// <param name="propertyValues">The property values to pass in to the template.</param>
    /// <param name="logLevel">The level of the log.</param>
    /// <param name="context">The context of the log.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    [Obsolete("Please don't use this needs a refactor to normalize the logs.")]
    public new Result<TValue> Log(
        [ConstantExpected] string messageTemplate = "",
        object?[]? propertyValues = null,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(Log)
    )
    {
        var template = "Context: {Context} - Result: {Result} - Message: " + messageTemplate;
        var allPropertyValues = new object?[(propertyValues?.Length ?? 0) + 2];
        allPropertyValues[0] = context;
        allPropertyValues[1] = ToString();
        propertyValues?.CopyTo(allPropertyValues, 2);

        Serilog.Log.Write(logLevel, template, allPropertyValues);
        return this;
    }

    /// <summary>
    /// Logs the result.
    /// </summary>
    /// <param name="isSuccess">Determines in which success state to log.</param>
    /// <param name="messageTemplate">The template of the message to log.</param>
    /// <param name="propertyValues">The property values to pass in to the template.</param>
    /// <param name="logLevel">The level of the log.</param>
    /// <param name="context">The context of the log.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    [Obsolete("Please don't use this needs a refactor to normalize the logs.")]
    public new Result<TValue> LogIf(
        bool isSuccess,
        [ConstantExpected] string messageTemplate,
        object?[]? propertyValues = null,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(LogIf)
    ) => IsSuccess == isSuccess ? Log(messageTemplate, propertyValues, logLevel, context) : this;

    /// <summary>
    /// Logs the result.
    /// </summary>
    /// <param name="messageTemplate">The template of the message to log.</param>
    /// <param name="propertyValuesSelector">
    /// The selectors to get the property values to pass in to the template.
    /// </param>
    /// <param name="logLevel">The level of the log.</param>
    /// <param name="context">The context of the log.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    [Obsolete("Please don't use this needs a refactor to normalize the logs.")]
    public Result<TValue> Log(
        [ConstantExpected] string messageTemplate,
        Func<Result<TValue>, object?[]> propertyValuesSelector,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(Log)
    )
    {
        var template = "Context: {Context} - Result: {Result} - Message: " + messageTemplate;
        var propertyValues = propertyValuesSelector(this);

        var allPropertyValues = new object?[propertyValues.Length + 2];
        allPropertyValues[0] = context;
        allPropertyValues[1] = ToString();
        propertyValues.CopyTo(allPropertyValues, 2);

        Serilog.Log.Write(logLevel, template, allPropertyValues);
        return this;
    }

    /// <summary>
    /// Logs the result.
    /// </summary>
    /// <param name="isSuccess">Determines in which success state to log.</param>
    /// <param name="messageTemplate">The template of the message to log.</param>
    /// <param name="propertyValues">
    /// The selectors to get the property values to pass in to the template.
    /// </param>
    /// <param name="logLevel">The level of the log.</param>
    /// <param name="context">The context of the log.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    [Obsolete("Please don't use this needs a refactor to normalize the logs.")]
    public Result<TValue> LogIf(
        bool isSuccess,
        [ConstantExpected] string messageTemplate,
        Func<Result<TValue>, object?[]> propertyValues,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(LogIf)
    ) => IsSuccess == isSuccess ? Log(messageTemplate, propertyValues, logLevel, context) : this;

    /// <summary>
    /// Wraps a function in a try-catch.
    /// </summary>
    /// <param name="func">The function to wrap.</param>
    /// <param name="exceptionHandler">Optional exception handler to transform the exception into an error.</param>
    /// <param name="context">The logging context.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    public static Result<TValue> Try(
        Func<TValue> func,
        Func<Exception, Error>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(Try)
    )
    {
        try
        {
            return Ok(func());
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <summary>
    /// Wraps a function in a try-catch.
    /// </summary>
    /// <param name="func">The function to wrap.</param>
    /// <param name="exceptionHandler">Optional exception handler to transform the exception into an error.</param>
    /// <param name="context">The logging context.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    public static async Task<Result<TValue>> TryAsync(
        Func<Task<TValue>> func,
        Func<Exception, Error>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(TryAsync)
    )
    {
        try
        {
            return Ok(await func());
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <summary>
    /// Wraps a function in a try-catch.
    /// </summary>
    /// <param name="func">The function to wrap.</param>
    /// <param name="exceptionHandler">Optional exception handler to transform the exception into an error.</param>
    /// <param name="context">The logging context.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    public static async Task<Result<TValue>> TryAsync(
        Func<ValueTask<TValue>> func,
        Func<Exception, Error>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(TryAsync)
    )
    {
        try
        {
            return Ok(await func());
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }
}
