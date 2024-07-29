using Connectivity.StartupExtensions;
using HealthChecks.UI.Client;
using HealthChecks.UI.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

namespace Student.StartupConfig;

internal static class WebApplicationWrapper
{
    public static WebApplication AddDefaultConfig(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapDefaultEndpoints();

        return app;
    }

    public static WebApplication AddHealthyCheck(this WebApplication app)
    {
        app.MapHealthChecks("/api/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecksUI(delegate(Options options)
        {
            options.UIPath = "/healthcheck-ui";
            options.AddCustomStylesheet("./Health/customHealthUi.css");
        });

        return app;
    }
}