using Common.Application.Extensions;
using Common.Application.Extensions.ServiceCollectionsExtensions;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Settings.Configuration;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.Spectre;

namespace Statistics.Logging.Extensions;

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddCustomSerilog(
        this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? extraConfigure = null,
        Action<SerilogOptions>? configurator = null
    )
    {
        var serilogOptions = builder.Configuration.BindOptions<SerilogOptions>();
        configurator?.Invoke(serilogOptions);

        // add option to the dependency injection
        builder.Services.AddConfigurationOptions<SerilogOptions>(opt => configurator?.Invoke(opt));

        builder.Logging.ClearProviders();
        builder.Services.AddSerilog(
            (sp, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(
                    builder.Configuration,
                    // Change default serilog configuration section from `Serilog` to `SerilogOptions`
                    new ConfigurationReaderOptions { SectionName = nameof(SerilogOptions) }
                );

                loggerConfiguration.ReadFrom.Configuration(
                    builder.Configuration,
                    new ConfigurationReaderOptions { SectionName = nameof(SerilogOptions) }
                );

                extraConfigure?.Invoke(loggerConfiguration);

                loggerConfiguration
                    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                    .Enrich.WithBaggage()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers());

                if (serilogOptions.UseConsole)
                {
                    loggerConfiguration.WriteTo.Async(writeTo =>
                        writeTo.Spectre(outputTemplate: serilogOptions.LogTemplate)
                    );
                }

                if (!string.IsNullOrEmpty(serilogOptions.ElasticSearchUrl))
                {
                    loggerConfiguration.WriteTo.Elasticsearch(
                        [new Uri(serilogOptions.ElasticSearchUrl),],
                        opts =>
                        {
                            opts.DataStream = new DataStreamName(
                                $"{
                                    builder.Environment.ApplicationName
                                }-{
                                    builder.Environment.EnvironmentName
                                }-{
                                    DateTime.Now
                                    :yyyy-MM}"
                            );

                            opts.BootstrapMethod = BootstrapMethod.Failure;

                            opts.ConfigureChannel = channelOpts =>
                            {
                                channelOpts.BufferOptions = new BufferOptions { ExportMaxConcurrency = 10 };
                            };
                        }
                    );
                }
                
                if (!string.IsNullOrEmpty(serilogOptions.GrafanaLokiUrl))
                {
                    loggerConfiguration.WriteTo.GrafanaLoki(
                        serilogOptions.GrafanaLokiUrl,
                        [
                            new LokiLabel { Key = "service", Value = "vertical-slice-template" },
                        ],
                        ["app"]
                    );
                }

                if (!string.IsNullOrEmpty(serilogOptions.SeqUrl))
                {
                    // seq sink internally is async
                    loggerConfiguration.WriteTo.Seq(serilogOptions.SeqUrl);
                }

                if (!string.IsNullOrEmpty(serilogOptions.LogPath))
                {
                    loggerConfiguration.WriteTo.Async(writeTo =>
                        writeTo.File(
                            serilogOptions.LogPath,
                            outputTemplate: serilogOptions.LogTemplate,
                            rollingInterval: RollingInterval.Day,
                            rollOnFileSizeLimit: true
                        )
                    );
                }
            },
            preserveStaticLogger: true,
            writeToProviders: serilogOptions.ExportLogsToOpenTelemetry
        );

        return builder;
    }
}
