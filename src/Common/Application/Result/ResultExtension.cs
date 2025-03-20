namespace Common.Application.Result;

/// <summary>
/// Partial class of <see cref="Result"/> for extensions.
/// </summary>
public partial class Result
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
    public Result Log(
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
    public Result LogIf(
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
    public Result Log(
        [ConstantExpected] string messageTemplate,
        Func<Result, object?[]> propertyValuesSelector,
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
    public Result LogIf(
        bool isSuccess,
        [ConstantExpected] string messageTemplate,
        Func<Result, object?[]> propertyValues,
        in LogEventLevel logLevel = LogEventLevel.Information,
        [CallerMemberName] string context = nameof(LogIf)
    ) => IsSuccess == isSuccess ? Log(messageTemplate, propertyValues, logLevel, context) : this;

    /// <summary>
    /// Wraps an action in a try-catch.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    /// <param name="exceptionHandler">Optional exception handler to transform the exception into an error.</param>
    /// <param name="context">The logging context.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    public static Result Try(
        Action action,
        Func<Exception, Error>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(Try)
    )
    {
        try
        {
            action();
            return Ok();
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <summary>
    /// Wraps an action in a try-catch.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    /// <param name="exceptionHandler">Optional exception handler to transform the exception into an error.</param>
    /// <param name="context">The logging context.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    public static async Task<Result> TryAsync(
        Func<Task> action,
        Func<Exception, Error>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(TryAsync)
    )
    {
        try
        {
            await action();
            return Ok();
        }
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <summary>
    /// Wraps an action in a try-catch.
    /// </summary>
    /// <param name="action">The action to wrap.</param>
    /// <param name="exceptionHandler">Optional exception handler to transform the exception into an error.</param>
    /// <param name="context">The logging context.</param>
    /// <returns>The <see cref="Result"/> reference.</returns>
    public static async Task<Result> TryAsync(
        Func<ValueTask> action,
        Func<Exception, Error>? exceptionHandler = null,
        [CallerMemberName] string context = nameof(TryAsync)
    )
    {
        try
        {
            await action();
            return Ok();
        }
            
        catch (Exception ex)
        {
            return Fail(exceptionHandler?.Invoke(ex) ?? new Error(ex.Message))
                .Log(logLevel: LogEventLevel.Error, context: context);
        }
    }

    /// <summary>
    /// Merges the results into a single result.
    /// </summary>
    /// <param name="results">The results to merge.</param>
    /// <returns>A <see cref="Result"/> with the merged errors or ok if all results are successful.</returns>
    public static Result MergeResults(params Result[] results)
    {
        if (results.Length == 0 || results.All(x => x.IsSuccess))
        {
            return Ok();
        }

        return Fail(results.SelectMany(x => x.Errors));
    }
}
