var builder = DistributedApplication.CreateBuilder(args);

var db = builder
    .AddPostgres("postgres")
    .WithPgWeb()
    .AddDatabase("keystoreDb");

var seq = builder
    .AddSeq("seq")
    .ExcludeFromManifest();

builder.AddProject<Projects.KeyStoreApi>("keystoreapi")
    .WithReference(db)
    .WithReference(seq);

builder
    .Build()
    .Run();
