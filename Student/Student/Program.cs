using Serilog;
using Student.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

Console.Title = builder.Configuration["ConsoleTitle"] ?? "Api";

builder.Host.UseSerilog((context, cfg) =>
{
    cfg.ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddInnerCommunication(builder.Configuration)
    .AddScopedCollection()
    .AddEndpointHub()
    .AddHealthyCheck(builder.Configuration);

var app = builder.Build();

app
    .AddDefaultConfig()
    .AddHealthyCheck();

app.Run();