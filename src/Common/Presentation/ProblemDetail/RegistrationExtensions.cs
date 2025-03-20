using System.Reflection;
using Common.Presentation.ProblemDetail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Statistics.Web.ProblemDetail;

public static class RegistrationExtensions
{
    public static IServiceCollection AddCustomProblemDetails(
        this IServiceCollection services,
        Action<ProblemDetailsOptions>? configure = null,
        params Assembly[] scanAssemblies
    )
    {
        var assemblies = scanAssemblies.Length != 0 ? scanAssemblies : [Assembly.GetCallingAssembly()];

        services.AddProblemDetails(configure);
        var descriptor = ServiceDescriptor.Singleton<IProblemDetailsService, ProblemDetailsService>();
        services.Replace(descriptor);
        services.AddSingleton<IProblemDetailsWriter, ProblemDetailsWriter>();

        return services;
    }
}
