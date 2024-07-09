using Connectivity.Configuration;
using MassTransit;
using Serilog;
using StudentRunner.Resources;

namespace StudentRunner.StartupConfig
{
    internal static class ServiceProviderWrapper
    {
        public static IServiceCollection AddInnerCommunication(this IServiceCollection cfg)
        {
            cfg.AddMassTransit(opt => 
            {
                opt.SetKebabCaseEndpointNameFormatter();
                opt.AddConsumers(typeof(Program).Assembly);

                opt.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(RabbitConfig.RabbitMqConnectionHost, RabbitConfig.RabbitMqConnectionVirtualHost, c =>
                    {
                        c.Username(RabbitConfig.RabbitMqUsername);
                        c.Password(RabbitConfig.RabbitMqPassword);
                    });
                    cfg.ConfigureEndpoints(ctx);
                });
            });

            return cfg;
        }

        public static IServiceCollection AddSerilogCustom(this IServiceCollection cfg, ConfigurationManager manager)
        {
            cfg.AddSerilog(ctx => ctx.ReadFrom.Configuration(manager));
            return cfg;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection cfg)
        {
            cfg.AddDbContext<Database>();
            return cfg;
        }
    }
}
