using Microsoft.EntityFrameworkCore;
using StudentRunner.Model;
using StudentRunner.Resources.TableConfig;

namespace StudentRunner.Resources;

public class Database: DbContext
{
    public static bool IsInTesting = false;
    private readonly string _dbConnectionString = "appsettings.json";
    //private readonly string _defaultConnectionString = "DefaultConnectionString";
    private readonly string _postgreConnectionString = "PostgreSqlConnectionString";
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