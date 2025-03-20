namespace Common.Presentation.ProblemDetail.Middlewares.CaptureExceptionMiddleware;

public class CaptureExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public CaptureExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            CaptureException(e, context);
            throw;
        }
    }

    private static void CaptureException(Exception exception, HttpContext context)
    {
        var instance = new ExceptionHandlerFeature { Path = context.Request.Path, Error = exception };
        context.Features.Set<IExceptionHandlerPathFeature>(instance);
        context.Features.Set<IExceptionHandlerFeature>(instance);
    }
}
