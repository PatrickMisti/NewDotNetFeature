using MassTransit;
using Student.Services;

namespace Student.StartupConfig;

internal static class ServiceProviderWrapper
{
    public static IServiceCollection AddScopedCollection(this IServiceCollection cfg)
    {
        cfg.AddScoped<IStudentService, StudentService>();
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
}