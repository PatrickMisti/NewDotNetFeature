using Microsoft.EntityFrameworkCore;
using Student.Resource.TableConfigurations;

namespace Student.Resource;

public class Database : DbContext
{
    private readonly string _dbConnectionString = "appsettings.json";
    private readonly string _defaultConnectionString = "DefaultConnectionString";
    public DbSet<Models.Student> Students { get; set; }

    public Database(DbContextOptions<Database> options) : base(options)
    {

    }

    public Database() : this(new DbContextOptionsBuilder<Database>().Options)
    {

    }

    // only is needed if there isn't configured in program file
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, _dbConnectionString))
            .Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString(_defaultConnectionString));
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
        builder.ApplyConfiguration(new BaseConfiguration<Models.Student>());
    }
}