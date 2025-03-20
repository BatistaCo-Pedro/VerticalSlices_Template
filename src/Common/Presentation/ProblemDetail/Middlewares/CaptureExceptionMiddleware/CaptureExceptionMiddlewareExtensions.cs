using Microsoft.AspNetCore.Builder;

namespace Statistics.Web.ProblemDetail.Middlewares.CaptureExceptionMiddleware;

public static class CaptureExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCaptureException(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.Properties["analysis.NextMiddlewareName"] =
            "Vertical.Slice.Template.Statistics.Web.Middlewares.CaptureExceptionMiddleware";
        return app.UseMiddleware<CaptureExceptionMiddleware>();
    }
}
