using Connectivity.Configuration;
using MassTransit;
using Serilog;
using Student.Resource;
using Student.Services;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddDbContext<Database>();
    builder.Services.AddScoped<IStudentService, StudentService>();
    builder.Host.UseSerilog((context, cfg) =>
    {
        cfg.ReadFrom.Configuration(context.Configuration);
    });

    builder.Services.AddMassTransit(opt =>
    {
        opt.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(RabbitConfig.RabbitMqConnectionHost, RabbitConfig.RabbitMqConnectionVirtualHost, c =>
            {
                c.Username(RabbitConfig.RabbitMqUsername);
                c.Password(RabbitConfig.RabbitMqPassword);
            });
            cfg.ConfigureEndpoints(context);
        });
    });
    var serviceBus = builder.Services.BuildServiceProvider();
    var bus = serviceBus.GetRequiredService<IBusControl>();
    await bus.StartAsync();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}

app.Run();