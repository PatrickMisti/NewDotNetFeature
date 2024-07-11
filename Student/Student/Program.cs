using Serilog;
using Student.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, cfg) =>
{
    cfg.ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddInnerCommunication(builder.Configuration)
    .AddScopedCollection()
    .AddEndpointHub();

var app = builder.Build();

app.AddDefaultConfig();

app.Run();