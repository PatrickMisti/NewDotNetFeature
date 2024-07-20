# Student

Easy Rest api with Ef core

## [Add Dependency Injection Methods](https://stackoverflow.com/questions/38138100/addtransient-addscoped-and-addsingleton-services-differences)

![alt text](Assets/Services.jpg)

### AddScoped

```c#
builder.Services.AddScoped<IStudentService, StudentService>();
```

Scoped lifetime services are created once per request.

### AddTransient

```c#
builder.Services.AddTransient<IStudentService, StudentService>();
```

Transient lifetime services are created each time they are requested. This lifetime works best for lightweight, stateless services.

### AddSingelton

```c#
builder.Services.AddSingelton<IStudentService, StudentService>();
```

Singleton lifetime services are created the first time they are requested (or when ConfigureServices is run if you specify an instance there) and then every subsequent request will use the same instance.


## Entityframework Core

There are some possibilities to implement DbContext

- Add DbContext to Startup file

```c#
builder.Services.AddDbContext<Database>(opt => //onConfiguring inDbContext
{
    var config = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(Environment.CurrentDirectory, dbConnectionString))
        .Build();
    opt.UseSqlServer(config.GetConnectionString(defaultConnectionString));
});

// or

builder.Configuration.GetConnectionString("DefaultConnectionString");

builder.Services.AddDbContext<Database>(opt => //onConfiguring inDbContext
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
    // use oder DB use
    opt.UseNpgSqlServer('context');
    //....
});
```

Configuration builder grab db context from appsettings.json

- Add by overriding onConfiguring in DbContext class

```c#
 protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, _dbConnectionString))
            .Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString(_defaultConnectionString));
        base.OnConfiguring(optionsBuilder);
    }
```

Or init in startup file

```c#
cfg.AddDbContext<Database>(opt =>
{
    opt
        .UseNpgsql(manager.GetConnectionString(PostGreConnectionString))
        .EnableSensitiveDataLogging();
});
```

### Create and Migrate Database ef
```bash
$ dotnet tool install --global dotnet-ef
$ dotnet add package Microsoft.EntityFrameworkCore.Design
$ dotnet ef migrations add InitialCreate
$ dotnet ef database update
```

## Add Serilog to WebApi and/or Host

```c#
builder.Host.UseSerilog((context, cfg) =>
{
    cfg.ReadFrom.Configuration(context.Configuration);
});
```

To read config add Serilog config to appsettings.json

```json
{
    "Serilog": {
        "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
        "MinimumLevel": {
            "Default": "Information",
            "Override": {
            "Microsoft": "Warning",
            "System": "Warning"
            }
        },
        "WriteTo": [
            { "Name": "Console" },
            {
                "Name": "File",
                "Args": {
                    "path": "Logs/log-development-.txt",
                    "rollingInterval": "Day",
                    "rollOnFileSizeLimit": true,
                    "formatter": "SerilogFormattingCompactCompactJsonFormatte,Serilog.Formatting.Compact" 
                }
            }
        ],
        "Enrich": [ 
            "FromLogContext", 
            "WithMachineName", 
            "WithProcessId", 
            "WithThreadId" 
        ],
        "Properties": {
            "Application": "Your ASP.NET Core App",
            "Environment": "Development"
        }
    }
}
```

## [Add Masstransit to Project](https://masstransit.io/quick-starts)

AddConsumers() with typof(Program) it scan entiry project with interface IConsumer.

To setup rabbitmq add host, username and password to transit.
By using Azur or AWS only include Masstransit package and update using.

ConfigureEndpoints() include all sender endoints.
```c#
cfg.AddMassTransit(opt =>
{
    opt.SetKebabCaseEndpointNameFormatter();
    opt.AddConsumers(typeof(Program).Assembly);

    opt.UsingRabbitMq((ctx, config) =>
    {
        config.Host(manager["Masstransit:Host"], "/", c =>
        {
            c.Username(manager["Masstransit:Username"]!);
            c.Password(manager["Masstransit:Password"]!);
        });

        config.ConfigureEndpoints(ctx);
    });
});
```

Masstransit config should be included in appsettings.json

```json
{
    "Masstransit": {
        "Host": "localhost",
        "Username": "guest",
        "Password": "guest"
    }
}
```


Benchmark
- only runs by running webapp

#### Before running application start docker file

```bash
$ docker-compose up
```