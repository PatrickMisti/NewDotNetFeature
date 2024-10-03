using KeyStoreApi.Persistence;
using KeyStoreApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddScoped<KeyStoreRepository>();
builder.Services.AddTransient<IKeyStoreService, KeyStoreService>();

builder.AddNpgsqlDbContext<KeyDbContext>("keystoreDb");

// For single docker service
/*builder.Services.AddDbContext<KeyDbContext>(opt =>
    opt.UseNpgsql(builder
        .Configuration
        .GetConnectionString("PostGreSqlConnectionString")));*/



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
