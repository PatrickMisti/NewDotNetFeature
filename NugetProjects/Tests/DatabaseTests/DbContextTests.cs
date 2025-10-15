using System.Data.Common;
using FhOoeProjectPackages.Database;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using SQLitePCL;
using Tests.DatabaseTests.Assets;

namespace Tests.DatabaseTests;

internal class DbContextTests
{
    DbConnection _connection = null!;
    private readonly string _connectionString = "Data Source=TestDb;Mode=Memory;Cache=Shared";

    [SetUp]
    public async Task Setup()
    {
        Batteries.Init();
        _connection = new SqliteConnection(_connectionString);
        await _connection.OpenAsync();
        //await CreateTable(_connection);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }

    [Test]
    public async Task AddAsync_Then_GetAllAsync_Works_With_Sqlite_InMemory()
    {
        await using var conn = new SqliteConnection(_connectionString);
        using var context = new DbContext(conn);
        var set = context.Set<DemoAttributeClass>();

        // Insert
        var entity = new DemoAttributeClass("hallo", 23, "ignored");
        var entity2 = new DemoAttributeClass("hallo", 24, "ignored");
        await set.AddAsync(entity);
        await set.AddAsync(entity2);
        //Assert.That(inserted, Is.Not.Empty);
        //Assert.That(inserted2, Is.Not.Empty);

        // Read back
        var all = (await set.GetAllAsync()).ToList();
        Assert.That(all.Count, Is.EqualTo(2));
        Assert.That(all[0].Id, Is.EqualTo(1));
        Assert.That(all[0].Name, Is.EqualTo("hallo"));
        Assert.That(all[0].Age, Is.EqualTo(23));
    }

    [Test]
    public async Task Check_Get_Value_By_Id()
    {
        await using var conn = new SqliteConnection(_connectionString);
        using var context = new DbContext(conn);
        var set = context.Set<DemoAttributeClass>();

        // Insert
        var entity = new DemoAttributeClass("hallo", 23, "ignored");
        var entity2 = new DemoAttributeClass("servus", 24, "ignored");
        await set.AddAsync(entity);
        await set.AddAsync(entity2);
        //Assert.That(inserted, Is.Not.Empty);
        //Assert.That(inserted2, Is.Not.Empty);

        var byId1 = await set.GetByIdAsync(1);
        Assert.Multiple(() =>
        {
            Assert.That(byId1, Is.Not.Null);
            Assert.That(byId1!.Name, Is.EqualTo(entity.Name));
            Assert.That(byId1!.Name, Is.Not.EqualTo(entity2.Name));
        });
    }

    [Test]
    public async Task Check_Add_And_Remove_Value()
    {
        await using var conn = new SqliteConnection(_connectionString);
        using var context = new DbContext(conn);
        var set = context.Set<DemoAttributeClass>();

        var entity = new DemoAttributeClass("hallo", 23, "ignored");
        await set.AddAsync(entity);

        var all = (await set.GetAllAsync()).ToList();
        Assert.That(all.Count, Is.EqualTo(1));

        var isDel = await set.DeleteByIdAsync(all.First().Id);
        Assert.That(isDel, Is.True);

        var allAfterDel = (await set.GetAllAsync()).ToList();
        Assert.That(allAfterDel.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task Check_Add_And_Update_Value()
    {
        await using var conn = new SqliteConnection(_connectionString);
        using var context = new DbContext(conn);
        var set = context.Set<DemoAttributeClass>();

        var entity = new DemoAttributeClass("hallo", 23, "ignored");
        await set.AddAsync(entity);
        var all = (await set.GetAllAsync()).ToList();
        Assert.That(all.Count, Is.EqualTo(1));

        var toUpdate = all.First();
        toUpdate.Name = "Seruvs";

        var updated = await set.UpdateAsync(toUpdate);

        Assert.Multiple(() =>
        {
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.Id, Is.EqualTo(toUpdate.Id));
            Assert.That(updated!.Name, Is.EqualTo("Seruvs"));
            Assert.That(updated!.Age, Is.EqualTo(23));
        });

    }
}