namespace Common.Infrastructure.EF.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresDbContext<TDbContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder>? builder = null
    )
        where TDbContext : DbContext
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", isEnabled: true);

        services.AddSingleton<AuditInterceptor>();
        services.AddSingleton<DomainEventInterceptor>();

        services.AddDbContext<TDbContext>(
            (sp, options) =>
            {
                var postgresOptions = sp.GetRequiredService<PostgresOptions>();

                options.UseNpgsql(
                    postgresOptions.ConnectionString,
                    sqlOptions =>
                    {
                        var name = typeof(TDbContext).Assembly.GetName().Name;

                        sqlOptions.MigrationsAssembly(name);
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), errorCodesToAdd: null);
                    }
                );

                options.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

                options.AddInterceptors(
                    sp.GetRequiredService<AuditInterceptor>(),
                    sp.GetRequiredService<DomainEventInterceptor>()
                );

                builder?.Invoke(options);
            }
        );

        return services;
    }
}
