var builder = DistributedApplication.CreateBuilder(args);

var db = builder
    .AddPostgres("postgres")
    .WithPgWeb()
    .AddDatabase("keystoreDb");

builder.AddProject<Projects.KeyStoreApi>("keystoreapi")
    .WithReference(db);

builder.Build().Run();
