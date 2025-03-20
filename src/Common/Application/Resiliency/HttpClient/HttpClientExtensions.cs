using System.Net;
using Common.Application.Extensions.ServiceCollectionsExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Statistics.Resiliency.Options;

namespace Statistics.Resiliency.HttpClient;

public static class HttpClientExtensions
{
    public static IServiceCollection AddCustomHttpClient<TClient, TImplementation, TClientOptions>(
        this IServiceCollection services,
        Action<IServiceProvider, System.Net.Http.HttpClient>? configureClient = null
    )
        where TClient : class
        where TImplementation : class, TClient
        where TClientOptions : HttpClientOptions, new()
    {
        services.AddValidatedOptions<TClientOptions>();

        services
            .AddHttpClient<TClient, TImplementation>()
            .ConfigureHttpClient(
                (serviceProvider, httpClient) =>
                {
                    var httpClientOptions = serviceProvider.GetRequiredService<IOptions<TClientOptions>>().Value;

                    var policyOptions = serviceProvider.GetRequiredService<IOptions<PolicyOptions>>().Value;

                    httpClient.BaseAddress = new Uri(httpClientOptions.BaseAddress);

                    httpClient.Timeout = TimeSpan.FromSeconds(policyOptions.TimeoutPolicyOptions.TimeoutInSeconds);

                    configureClient?.Invoke(serviceProvider, httpClient);
                }
            )
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler()
            {
                AutomaticDecompression =
                    DecompressionMethods.Brotli | DecompressionMethods.Deflate | DecompressionMethods.GZip,
            });

        return services;
    }

    public static IServiceCollection AddCustomHttpClient<TClient, TClientOptions>(
        this IServiceCollection services,
        Action<IServiceProvider, System.Net.Http.HttpClient>? configureClient = null
    )
        where TClient : class
        where TClientOptions : HttpClientOptions, new()
    {
        return services.AddCustomHttpClient<TClient, TClient, TClientOptions>(configureClient);
    }

    public static IServiceCollection AddCustomHttpClient<TClientOptions>(
        this IServiceCollection services,
        string clientName,
        Action<IServiceProvider, System.Net.Http.HttpClient>? configureClient = null
    )
        where TClientOptions : HttpClientOptions, new()
    {
        services.AddValidatedOptions<TClientOptions>();

        services
            .AddHttpClient(clientName)
            .ConfigureHttpClient(
                (sp, httpClient) =>
                {
                    var httpClientOptions = sp.GetRequiredService<IOptions<TClientOptions>>().Value;
                    var policyOptions = sp.GetRequiredService<IOptions<PolicyOptions>>().Value;
                    httpClient.BaseAddress = new Uri(httpClientOptions.BaseAddress);
                    httpClient.Timeout = TimeSpan.FromSeconds(policyOptions.TimeoutPolicyOptions.TimeoutInSeconds);

                    configureClient?.Invoke(sp, httpClient);
                }
            )
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler()
            {
                AutomaticDecompression =
                    DecompressionMethods.Brotli | DecompressionMethods.Deflate | DecompressionMethods.GZip,
            });

        return services;
    }
}
