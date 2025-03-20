namespace Common.Presentation.ProblemDetail;

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
