using StudentRunner.StartupConfig;

var builder = Host.CreateApplicationBuilder(args);

//builder.AddServiceDefaults();

builder.Services
    .AddInnerCommunication(builder.Configuration)
    .AddSerilogCustom(builder.Configuration)
    .AddDatabase(builder.Configuration);

var app = builder.Build();

app.Run();
