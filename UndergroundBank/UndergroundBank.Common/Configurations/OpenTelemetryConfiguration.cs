using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace UndergroundBank.Common.Configurations;

public static class OpenTelemetryConfiguration
{
    public static WebApplicationBuilder AddOpenTelemetry(
        this WebApplicationBuilder builder,
        string serviceName
    )
    {
        var collector = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "C:\\Users\\Артем\\Source\\Repos\\Underground-Backend\\UndergroundBank\\UndergroundBank.Common\\Configurations\\otelcol-contrib.exe",
                Arguments = "--config C:\\Users\\Артем\\Source\\Repos\\Underground-Backend\\UndergroundBank\\UndergroundBank.Common\\Configurations\\otel-collector-config.yaml",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        collector.OutputDataReceived += (s, e) => Console.WriteLine("[OTEL] " + e.Data);
        collector.ErrorDataReceived += (s, e) => Console.WriteLine("[OTEL ERROR] " + e.Data);

        collector.Start();
        collector.BeginOutputReadLine();
        collector.BeginErrorReadLine();


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
                o.Endpoint = new Uri("http://localhost:4318");
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
                        options.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/swagger");
                    })
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:4318");
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
                        options.Endpoint = new Uri("http://localhost:4318");
                    })
            );

        return builder;
    }
}
