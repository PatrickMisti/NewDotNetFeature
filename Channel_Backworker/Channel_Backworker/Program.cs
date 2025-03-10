var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(); // check if needed
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // check if needed
    app.UseSwagger(opt =>
    {
        opt.SerializeAsV2 = true;
    });

    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        opt.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
