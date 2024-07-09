using Serilog;
using Student.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, cfg) =>
{
    cfg.ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddScopedCollection()
    .AddInnerCommunication()
    .AddEndpointHub();

var app = builder.Build();

app.AddDefaultConfig();

app.Run();