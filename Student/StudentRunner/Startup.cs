using StudentRunner.Resources;using StudentRunner.StartupConfig;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInnerCommunication().AddSerilogCustom(builder.Configuration).AddDatabase();

var app = builder.Build();

// only for testing
new Database().InitDb();

app.Run();
