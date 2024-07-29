using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Connectivity.StartupExtensions;

public static class ConfigExtensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();

        Console.Title = builder.Configuration["ConsoleTitle"] ?? "Api";

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(opt =>
        {
            opt.IncludeScopes = true;
            opt.IncludeFormattedMessage = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(opt =>
            {
                opt.AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter()
                    .AddMeter(
                        "Microsoft.AspNetCore.Hosting", 
                        "Microsoft.AspNetCore.Server.Kestrel", 
                        "System.Net.Http");
            })
            .WithTracing(opt =>
            {
                if (builder.Environment.IsDevelopment())
                    opt.SetSampler<AlwaysOffSampler>();

                opt
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());
        }

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        //app.UseOpenTelemetryPrometheusScrapingEndpoint();
        app.MapPrometheusScrapingEndpoint();

        return app;
    }
}