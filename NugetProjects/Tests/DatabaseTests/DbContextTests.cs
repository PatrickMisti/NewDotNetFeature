using FhOoeProjectPackages.Database;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Tests.DatabaseTests.AttributeTests.Assets;

namespace Tests.DatabaseTests;

internal class DbContextTests
{
    [Test]
    public void SampleTest()
    {
        using var db = new SqliteConnection("Data Source=:memory:");
        using var context = new DbContext(db);
        var demoSet = context.Set<DemoAttributeClass>();

        demoSet.Add(new DemoAttributeClass("hallo", 23, ""));
        var allItems = demoSet.GetAll<DemoAttributeClass>().ToList();

        Assert.That(allItems, Is.EqualTo(1));
    }
}