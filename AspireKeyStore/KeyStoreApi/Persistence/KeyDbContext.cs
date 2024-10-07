using KeyStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyStoreApi.Persistence;

public class KeyDbContext : DbContext
{
    private DbSet<KeyEntry> KeyEntries { get; }

    // region Constructor
    public KeyDbContext(DbContextOptions<KeyDbContext> options) : base(options)
    {

    }

    public KeyDbContext() : this(new DbContextOptionsBuilder<KeyDbContext>().Options)
    {

    }
    // endregion 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KeyEntry>();
        base.OnModelCreating(modelBuilder);
    }
}