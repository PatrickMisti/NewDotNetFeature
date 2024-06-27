using Microsoft.EntityFrameworkCore;
using Student.Resource;
using Student.Services;

const string dbConnectionString = "appsettings.json";
var defaultConnectionString = "DefaultConnectionString";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<Database>(opt =>
{
    var config = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(Environment.CurrentDirectory, dbConnectionString))
        .Build();
    opt.UseSqlServer(config.GetConnectionString(defaultConnectionString));
});
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

/*Transient
   
   Transient lifetime services are created each time they are requested. This lifetime works best for lightweight, stateless services.
   
   Scoped
   
   Scoped lifetime services are created once per request.
   
   Singleton
   
   Singleton lifetime services are created the first time they are requested (or when ConfigureServices is run if you specify an instance there) and then every subsequent request will use the same instance.*/