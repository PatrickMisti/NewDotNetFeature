using System.Data.Common;
using FhOoeProjectPackages.Database;
using FhOoeProjectPackages.Database.Utilities;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Tests.DatabaseTests.Assets;

namespace Tests.DatabaseTests;

[TestFixture]
[Parallelizable(ParallelScope.None)]
public class DbContextTests
{
    private readonly string _connectionString = "Data Source=TestDb;Mode=Memory;Cache=Shared";
    private readonly string _providerString = "Microsoft.Data.Sqlite";
    private DbContext _context;
    private SqliteConnection _keeperConnection;

    [SetUp]
    public async Task Setup()
    {
        _context = new DbContext(_providerString, _connectionString);

        // Open and keep this connection alive until TearDown
        _keeperConnection = new SqliteConnection(_connectionString);
        await _keeperConnection.OpenAsync();

        await using var cmd = _keeperConnection.CreateCommand();
        cmd.CommandText = DatabaseUtils.GenerateTableStmt<DemoAttributeClass>(providerName: _providerString);
        await cmd.ExecuteNonQueryAsync();
        cmd.CommandText = DatabaseUtils.GenerateTableStmt<DemoAttributeClassWithCustomTableName>(_providerString);
        await cmd.ExecuteNonQueryAsync();
        cmd.CommandText = DatabaseUtils.GenerateTableStmt<DemoAttributeAsChildClass>(_providerString);
        await cmd.ExecuteNonQueryAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _keeperConnection.Close();
    }

    [Test]
    public void Test_Table_Generator()
    {
        var i = DatabaseUtils.GenerateTableStmt<DemoAttributeAsChildClass>(_providerString);

        Assert.That(i, Is.Not.Null);
    }

    [Test]
    public async Task AddAsync_Then_GetAllAsync_Works_With_Sqlite_InMemory()
    {
        var set = _context.Set<DemoAttributeClass>();

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
        var set = _context.Set<DemoAttributeClass>();

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
        var set = _context.Set<DemoAttributeClass>();

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
        var set = _context.Set<DemoAttributeClass>();

        var entity = new DemoAttributeClass("hallo", 23, "ignored");
        await set.AddAsync(entity);
        var all = (await set.GetAllAsync()).ToList();
        Assert.That(all.Count, Is.EqualTo(1));

        var toUpdate = all.First();
        toUpdate.Name = "Seruvs";

        var updated = await set.UpdateAsync(toUpdate);

        Assert.That(updated, Is.True);
    }

    [Test]
    public async Task Check_Eagle_Loading_From_OneToMany()
    {
        var set = _context.Set<DemoAttributeClassWithCustomTableName>();
        var setChild = _context.Set<DemoAttributeAsChildClass>();

        var entity = new DemoAttributeClassWithCustomTableName
        {
            Name = "ParentEntity",
            Children = new List<DemoAttributeAsChildClass>
            {
                new DemoAttributeAsChildClass { Description = "Child 1" },
                new DemoAttributeAsChildClass { Description = "Child 2" }
            }
        };
        var entity2 = new DemoAttributeClassWithCustomTableName
        {
            Name = "ParentEntity2",
            Children = new List<DemoAttributeAsChildClass>
            {
                new DemoAttributeAsChildClass { Description = "Child 3" },
                new DemoAttributeAsChildClass { Description = "Child 4" }
            }
        };

        entity.Id = (int)(await set.AddAsync(entity))!;
        entity2.Id = (int)(await set.AddAsync(entity2))!;
        foreach (var e in entity.Children)
        {
            e.ParentId = entity.Id;
            await setChild.AddAsync(e);
        }
        foreach (var e in entity2.Children)
        {
            e.ParentId = entity2.Id;
            await setChild.AddAsync(e);
        }

        var loadedEntity = await set.GetByIdAsync(entity.Id, enableEagerLoading: true);

        Assert.Multiple(() =>
        {
            Assert.That(loadedEntity, Is.Not.Null);
            Assert.That(loadedEntity!.Children, Is.Not.Null);
            Assert.That(loadedEntity.Children!.Count, Is.EqualTo(2));
            Assert.That(loadedEntity.Children![0].Description, Is.EqualTo("Child 1"));
            Assert.That(loadedEntity.Children![1].Description, Is.EqualTo("Child 2"));
        });
    }
}