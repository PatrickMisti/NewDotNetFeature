using StudentRunner.StartupConfig;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInnerCommunication();

var app = builder.Build();

app.Run();
