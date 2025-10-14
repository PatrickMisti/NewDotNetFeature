using System.Reflection;
using FhOoeProjectPackages.Database.DbAttributes;
using NUnit.Framework;
using Tests.DatabaseTests.AttributeTests.Assets;

namespace Tests.DatabaseTests.AttributeTests;

public class AttributeTests
{

    [Test]
    public void TableAttribute_Should_Set_Name_Property()
    {
        // Arrange
        var tableName = "TestTable";
        var tableAttribute = new TableAttribute();
        // Act
        var nameProperty = tableAttribute.Name;
        // Assert
        Assert.That(nameProperty, Is.EqualTo(tableName));
    }
    [Test]
    public void ColumnAttribute_Should_Set_Properties_Correctly()
    {
        // Arrange
        var columnName = "TestColumn";
        var columnAttribute = new ColumnAttribute
        {
            IsPrimaryKey = true,
            IsForeignKey = false,
            IsNullable = false,
            AutoIncrement = true
        };
        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(columnAttribute.Name, Is.EqualTo(columnName));
            Assert.That(columnAttribute.IsPrimaryKey, Is.True);
            Assert.That(columnAttribute.IsForeignKey, Is.False);
            Assert.That(columnAttribute.IsNullable, Is.False);
            Assert.That(columnAttribute.AutoIncrement, Is.True);
        });
    }

    [Test]
    public void KeyAttribute_Should_Set_Properties_Correctly()
    {
        // Arrange
        var keyName = "TestKey";
        var keyAttribute = new KeyAttribute
        {
            Name = keyName,
            AutoIncrement = false
        };
        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(keyAttribute.Name, Is.EqualTo(keyName));
            Assert.That(keyAttribute.AutoIncrement, Is.False);
        });
    }

    [Test]
    public void Attribute_From_Class_Should_Work()
    {
        var e = new DemoAttributeClass("Herber", 23, "ignore");

        var type = typeof(DemoAttributeClass);

        var table = type.GetCustomAttribute<TableAttribute>();
        Assert.That(table?.Name, Is.EqualTo("DemoAttributeClass"));

        var props = type.GetProperties();

        var keyProp = props.FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);
        var nameColumn = props.FirstOrDefault(p => p.GetCustomAttribute<ColumnAttribute>()?.Name == "DemoColumn");
        var ageColumn = props.FirstOrDefault(p => p.GetCustomAttribute<ColumnAttribute>()?.Name == "Age");

        Assert.Multiple(() =>
        {
            Assert.That(keyProp?.Name, Is.EqualTo(nameof(DemoAttributeClass.Id)));
            Assert.That(nameColumn?.GetValue(e), Is.EqualTo(e.Name));
            Assert.That(ageColumn?.GetValue(e), Is.EqualTo(e.Age));
            Assert.That(e.IgnoredProperty, Is.EqualTo(e.IgnoredProperty));
        });
    }

    [Test]
    public void Attribute_Demo()
    {
        var e = new DemoAttributeClass("Herber", 23, "ignore");

        var type = e.GetType();
        var type2 = typeof(DemoAttributeClass);

        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var props2 = type2.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var key = props.FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() is not null);

        var i = key?.Name;
        var s = key?.GetCustomAttribute<KeyAttribute>()?.Name;
        
        var s_type = type2.GetCustomAttribute<KeyAttribute>();

        Console.WriteLine("Tests");
    }
}