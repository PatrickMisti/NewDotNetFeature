
using Connectivity.Configuration;
using MassTransit;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(opt =>
{
    opt.SetKebabCaseEndpointNameFormatter();

    var entryAssembly = Assembly.GetExecutingAssembly();
    opt.AddConsumers(entryAssembly);

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

var app = builder.Build();
app.Run();
