using System.Data.Common;

namespace FhOoeProjectPackages.Database;

public class DbContext(DbConnection connection) : IDisposable
{

    public DbSet<T> Set<T>() where T : class, new() => new (connection);
    
    public void Dispose() => connection.Dispose();
}