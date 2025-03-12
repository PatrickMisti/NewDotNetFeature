using Channel_Backworker.Config;
using Student_Coordinator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services
    .AddSwagger()
    .AddExternalServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // check if needed
    app.UseSwaggerConf();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
