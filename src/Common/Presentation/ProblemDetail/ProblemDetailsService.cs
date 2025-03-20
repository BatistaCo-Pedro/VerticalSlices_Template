using Humanizer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Statistics.Web.ProblemDetail;

public class ProblemDetailsService(IEnumerable<IProblemDetailsWriter> writers) : IProblemDetailsService
{
    private readonly IProblemDetailsWriter[] _writers = writers.ToArray();

    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(context.ProblemDetails);
        ArgumentNullException.ThrowIfNull(context.HttpContext);

        var exceptionFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();

        if (exceptionFeature is not null)
        {
            PopulateNewProblemDetail(context.ProblemDetails, context.HttpContext, exceptionFeature.Error);
        }

        if (
            context.HttpContext.Response.HasStarted
            || context.HttpContext.Response.StatusCode < 400
            || _writers.Length == 0
        )
        {
            return ValueTask.CompletedTask;
        }

        if (_writers.Length == 1)
        {
            var writer = _writers[0];
            return !writer.CanWrite(context) ? ValueTask.CompletedTask : writer.WriteAsync(context);
        }

        var problemDetailsWriter = _writers.FirstOrDefault(writer => writer.CanWrite(context));

        return problemDetailsWriter?.WriteAsync(context) ?? ValueTask.CompletedTask;
    }

    private static void PopulateNewProblemDetail(
        ProblemDetails existingProblemDetails,
        HttpContext httpContext,
        Exception exception
    )
    {
        existingProblemDetails.Title = exception.GetType().Name.Humanize(LetterCasing.Title);
        existingProblemDetails.Detail = exception.Message;
        existingProblemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
    }
}
