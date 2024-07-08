
using Connectivity.Configuration;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(opt =>
{
    opt.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(RabbitConfig.RabbitMqConnectionHost, RabbitConfig.RabbitMqConnectionVirtualHost, c =>
        {
            c.Username(RabbitConfig.RabbitMqUsername);
            c.Password(RabbitConfig.RabbitMqPassword);
        });
    });
});

var app = builder.Build();
app.Run();
