namespace Common.Presentation.ProblemDetail;

public class ProblemDetailsWriter : IProblemDetailsWriter
{
    private readonly ProblemDetailsOptions _options;

    public ProblemDetailsWriter(IOptions<ProblemDetailsOptions> options)
    {
        _options = options.Value;
    }

    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        var httpContext = context.HttpContext;
        TypedResults.Problem(context.ProblemDetails);
        _options.CustomizeProblemDetails?.Invoke(context);

        return new ValueTask(
            httpContext.Response.WriteAsJsonAsync(
                context.ProblemDetails,
                options: null,
                contentType: "application/problem+json"
            )
        );
    }

    public bool CanWrite(ProblemDetailsContext context)
    {
        return true;
    }
}
