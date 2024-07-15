using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StudentRunner.Resources;

namespace StudentRunner.StartupConfig;

internal static class ServiceProviderWrapper
{
    private static readonly string PostGreConnectionString = "PostGreSqlConnectionString";
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

    public static IServiceCollection AddSerilogCustom(this IServiceCollection cfg, ConfigurationManager manager)
    {
        cfg.AddSerilog(ctx => ctx.ReadFrom.Configuration(manager));
        return cfg;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection cfg, ConfigurationManager manager)
    {
        cfg.AddSingleton<IStudentRepository, StudentRepository>();

        cfg.AddDbContext<Database>(opt =>
        {
            opt
                .UseNpgsql(manager.GetConnectionString(PostGreConnectionString))
                .EnableSensitiveDataLogging();
        });
        
        return cfg;
    }
}