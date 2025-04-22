using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace UndergroundBank.Common.Configurations;

public static class OpenTelemetryConfiguration
{
    public static WebApplicationBuilder AddOpenTelemetry(
        this WebApplicationBuilder builder,
        string serviceName
    )
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;

            options.SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService(serviceName: serviceName)
            );

            options.AddOtlpExporter(o =>
            {
                o.Endpoint = new Uri("http://localhost:5035/api/monitoring/logs");
            });
        });

        var meter = new Meter($"{serviceName}.Metrics", "1.0.0");
        var successCounter = meter.CreateCounter<long>("requests_success_total");
        var errorCounter = meter.CreateCounter<long>("requests_error_total");

        builder
            .Services.AddOpenTelemetryTracing(builder =>
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:5035/api/monitoring/traces");
                    })
            )
            .AddOpenTelemetryMetrics(builder =>
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddMeter($"{serviceName}.Metrics")
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:5035/api/monitoring/logs");
                    })
            );

        return builder;
    }
}
