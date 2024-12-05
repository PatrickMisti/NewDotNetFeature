using Connection;
using Connection.Services;
using Connectivity.Configuration;
using MassTransit;
using Student.Services;

namespace Student.StartupConfig;

internal static class ServiceProviderWrapper
{
    public static IServiceCollection AddScopedCollection(this IServiceCollection cfg)
    {
        cfg.AddScoped<IStudentService, StudentService>();
        //cfg.AddSingleton<StudentDbContext>();
        return cfg;
    }

    public static IServiceCollection AddDatabaseContext(this IServiceCollection cfg)
    {
        cfg.AddDbContext<Database>();
        return cfg;
    }

    public static IServiceCollection AddInnerCommunication(this IServiceCollection cfg)
    {
        cfg.AddMassTransit(opt =>
        {
            opt.SetKebabCaseEndpointNameFormatter();

            opt.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(RabbitConfig.RabbitMqConnectionHost, RabbitConfig.RabbitMqConnectionVirtualHost, c =>
                {
                    c.Username(RabbitConfig.RabbitMqUsername);
                    c.Password(RabbitConfig.RabbitMqPassword);
                });
                //cfg.ConfigureEndpoints(context);
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