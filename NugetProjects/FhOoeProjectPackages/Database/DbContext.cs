using System.Data;

namespace FhOoeProjectPackages.Database;

public class DbContext(IDbConnection connection) : IDisposable
{
    public DbSet<T> Set<T>() where T : class => new (connection);
    
    public void Dispose() => connection.Dispose();
}