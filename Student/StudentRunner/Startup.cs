using StudentRunner.StartupConfig;

var builder = Host.CreateApplicationBuilder(args);

Console.Title = builder.Configuration["ConsoleTitle"] ?? "Runner";

builder.Services
    .AddInnerCommunication(builder.Configuration)
    .AddSerilogCustom(builder.Configuration)
    .AddDatabase(builder.Configuration);

var app = builder.Build();

app.Run();
