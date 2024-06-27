using Microsoft.EntityFrameworkCore;
using Student.Resource.TableConfigurations;

namespace Student.Resource;

public class Database(DbContextOptions<Database> options) : DbContext(options)
{
    public DbSet<Models.Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        InitializeTable(modelBuilder);
        modelBuilder.Entity<Models.Student>().HasQueryFilter(opt => !opt.Deleted);
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