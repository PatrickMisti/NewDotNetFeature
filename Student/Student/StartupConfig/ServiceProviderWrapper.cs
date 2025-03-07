using Connection;
using Connection.Services;
using Connectivity.Configuration;
using MassTransit;
<<<<<<< HEAD
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Student.Health;
=======
>>>>>>> origin/other
using Student.Services;

namespace Student.StartupConfig;

internal static class ServiceProviderWrapper
{
    public static IServiceCollection AddScopedCollection(this IServiceCollection cfg)
    {
        cfg.AddScoped<IStudentService, StudentService>();
<<<<<<< HEAD
        cfg.AddSingleton<PubSub>();
=======
        //cfg.AddSingleton<StudentDbContext>();
>>>>>>> origin/other
        return cfg;
    }

    public static IServiceCollection AddInnerCommunication(this IServiceCollection cfg, ConfigurationManager manager)
    {
        cfg.AddMassTransit(opt =>
        {
            opt.SetKebabCaseEndpointNameFormatter();
            opt.AddConsumers(typeof(Program).Assembly);

            opt.UsingRabbitMq((ctx, config) =>
            {
                config.Host(manager["Masstransit:Host"], "/", c =>
                {
                    c.Username(manager["Masstransit:Username"]!);
                    c.Password(manager["Masstransit:Password"]!);
                });

                config.ConfigureEndpoints(ctx);
            });
        });

        return cfg;
    }

    public static IServiceCollection AddEndpointHub(this IServiceCollection cfg)
    {
        cfg.AddControllers();
        cfg.AddEndpointsApiExplorer();
        cfg.AddSwaggerGen();

        return cfg;
    }

    public static IServiceCollection AddHealthyCheck(this IServiceCollection cfg, ConfigurationManager manager)
    {
        var massUser = manager["Masstransit:Username"];
        var massPass = manager["Masstransit:Password"];
        var massHost = manager["Masstransit:Host"];

        cfg
            .AddHealthChecks()
            .AddNpgSql(
                manager.GetConnectionString("PostGreSqlConnectionString")!,
                healthQuery: "select 1",
                name: "PostGre Server",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["Feedback", "Database"])
            .AddRabbitMQ(
                rabbitConnectionString: $"amqp://{massUser}:{massPass}@{massHost}:5672",
                name: "Rabbit MQ",
                failureStatus: HealthStatus.Unhealthy)
            .AddCheck<MemoryHealthCheck>(
                "Feedback Service Memory Check",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["Feedback Service"]);
#if DEBUG

        cfg.AddHealthChecksUI(opt =>
        {
            opt.SetEvaluationTimeInSeconds(10);                                      // time in seconds between check    
            opt.MaximumHistoryEntriesPerEndpoint(60);                                // maximum history of checks 
            opt.SetApiMaxActiveRequests(1);                                          // api requests concurrency    
            opt.AddHealthCheckEndpoint("feedback api", "/api/health");      // map health check api    
        })
            .AddInMemoryStorage();
#endif

        return cfg;
    }
}