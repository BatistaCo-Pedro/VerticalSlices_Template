namespace Common.Application.Result.Errors;

/// <summary>
/// Default implementation of the <see cref="IError"/> interface.
/// </summary>
[Serializable]
public class Error : IError
{
    /// <summary>
    /// Represents an empty error.
    /// </summary>
    public static Error Empty { get; } = new();

    /// <inheritdoc />
    [JsonPropertyName("message")]
    public string Message { get; }

    /// <inheritdoc />
    [JsonPropertyName("metadata")]
    public ImmutableDictionary<string, string> Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    private Error()
        : this(string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/>.
    /// </summary>
    /// <param name="message">The error message to set.</param>
    /// <param name="metadata">The metadata to set.</param>
    [JsonConstructor]
    protected Error(string message, ImmutableDictionary<string, string> metadata)
    {
        Message = message;
        Metadata = metadata;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/>
    /// class with the specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public Error(string message)
    {
        Message = message;
        Metadata = ImmutableDictionary<string, string>.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/>
    /// class with the specified error message.
    /// </summary>
    /// <param name="exception">The error message.</param>
    public Error(Exception exception)
    {
        Message = exception.Message;
        Metadata = new Dictionary<string, string>
        {
            { "StackTrace", exception.StackTrace ?? string.Empty },
            { "Source", exception.Source ?? string.Empty },
            { "TargetSite", exception.TargetSite?.Name ?? string.Empty },
        }.ToImmutableDictionary();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/>
    /// class with the specified error message and metadata.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="metadata">The metadata associated with the error.</param>
    public Error(string message, (string Key, object Value) metadata)
    {
        Message = message;
        Metadata = new Dictionary<string, string>
        {
            { metadata.Key, metadata.Value.ToString()! },
        }.ToImmutableDictionary();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/>
    /// class with the specified error message and metadata.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="metadata">The metadata associated with the error.</param>
    public Error(string message, IDictionary<string, string> metadata)
    {
        Message = message;
        Metadata = metadata.ToImmutableDictionary();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/>
    /// class with the specified error message and metadata.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="metadata">The metadata associated with the error.</param>
    public Error(string message, IDictionary<string, object> metadata)
    {
        Message = message;
        Metadata = metadata.ToImmutableDictionary(x => x.Key, x => x.Value.ToString()!);
    }

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this);
}
