using Connection.Models;
using Microsoft.EntityFrameworkCore;
using Connection.TableConfigurations;
using Microsoft.Extensions.Configuration;

namespace Connection;

public class Database : DbContext
{
    private readonly string _dbConnectionString = "appsettings.json";
    private readonly string _defaultConnectionString = "DefaultConnectionString";
    private readonly string _postGreConnectionString = "PostgreSqlConnectionString";

    public DbSet<Student> Students { get; set; }

    public Database(DbContextOptions<Database> options) : base(options)
    {

    }

    public Database() : this(new DbContextOptionsBuilder<Database>().Options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, _dbConnectionString))
            .Build();

        //optionsBuilder.UseSqlServer(config.GetConnectionString(_defaultConnectionString));
        optionsBuilder.UseNpgsql(config.GetConnectionString(_postGreConnectionString));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        InitializeTable(modelBuilder);
        AddConfigurations(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void InitializeTable(ModelBuilder model)
    {
        model.Entity<Models.Student>();
    }

    private void AddConfigurations(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new BaseConfiguration<Student>());
    }
}