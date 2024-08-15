var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireKeyStore_ApiService>("apiservice");

builder.AddProject<Projects.AspireKeyStore_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
