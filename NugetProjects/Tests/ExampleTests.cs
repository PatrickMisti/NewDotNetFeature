using FhOoeProjectPackages.Database;
using NUnit.Framework;
using Tests.DatabaseTests.AttributeTests.Assets;
using Microsoft.Data.Sqlite;

namespace Tests;

public class ExampleTests
{
    [Test]
    public void SampleTest()
    {
        using var sql = new SqliteConnection();
        var db = new DbSet<DemoAttributeClass>(sql);
        Assert.That(2, Is.EqualTo(1 + 1));
    }

}