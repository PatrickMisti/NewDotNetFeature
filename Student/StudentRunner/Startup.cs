using StudentRunner.Resources;using StudentRunner.StartupConfig;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddInnerCommunication(builder.Configuration)
    .AddSerilogCustom(builder.Configuration)
    .AddDatabase(builder.Configuration);

var app = builder.Build();

app.Run();
