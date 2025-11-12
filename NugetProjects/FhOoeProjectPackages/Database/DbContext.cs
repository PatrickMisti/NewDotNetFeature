using System.Data.Common;
using FhOoeProjectPackages.Database.Utilities;

namespace FhOoeProjectPackages.Database;

public class DbContext : IDisposable
{
    private readonly DbConnection _connection;
    public DbContext(string providerName, string connectionString)
    {
        if (providerName == "Microsoft.Data.Sqlite")
        {
            DbProviderRegistration.EnsureSqliteRegistered();
        }

        var factory = DbProviderFactories.GetFactory(providerName);
        _connection = factory.CreateConnection() ?? throw new ArgumentException("Could not create a connection with the provided provider name.");
        _connection.ConnectionString = connectionString;
    }

    public DbSet<T> Set<T>() where T : class, new() => new (_connection);
    
    public void Dispose() => _connection.Dispose();
}