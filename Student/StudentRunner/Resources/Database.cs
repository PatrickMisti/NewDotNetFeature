using Microsoft.EntityFrameworkCore;
using StudentRunner.Model;
using StudentRunner.Resources.TableConfig;

namespace StudentRunner.Resources;

public class Database: DbContext
{
    private const string AppSettingsString = "appsettings.json";

    public DbSet<Student> Students { get; set; }

    public Database(DbContextOptions<Database> options) : base(options)
    {

    }

    // only is needed if there isn't configured in program file
    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, _dbConnectionString))
            .Build();
        //optionsBuilder.UseSqlServer(config.GetConnectionString(_defaultConnectionString));
        optionsBuilder.UseNpgsql(config.GetConnectionString(_postgreConnectionString));
        base.OnConfiguring(optionsBuilder);
    }*/

    public static DbContextOptionsBuilder<Database> GetTestTableConnectionString()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, AppSettingsString))
            .Build();

        var dbConfig = config["ConnectionStrings:PostGreSqlTestConnectionString"]!;

        var builder = new DbContextOptionsBuilder<Database>();
        builder.UseNpgsql(dbConfig).EnableSensitiveDataLogging();
        return builder;
    }

    public void InitDb()
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        InitializeTable(modelBuilder);
        AddConfigurations(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void InitializeTable(ModelBuilder model)
    {
        model.Entity<Student>();
    }

    private void AddConfigurations(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new BaseConfiguration<Student>());
    }
}