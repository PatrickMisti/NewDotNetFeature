using FhOoeProjectPackages.Database.DbAttributes;

namespace Tests.DatabaseTests.Assets;

[Table]
public class DemoAttributeClass()
{
    [Key(Name = "DemoKey")]
    public int Id { get; set; }

    [Column(Name = "DemoColumn")] 
    public string Name { get; set; } = string.Empty;

    [Column]
    public int Age { get; set; }

    public string IgnoredProperty { get; set; } = string.Empty;

    public DemoAttributeClass(string name, int age, string ignored) : this()
    {
        Name = name;
        Age = age;
        IgnoredProperty = ignored;
    }
}

[Table(Name = "CustomTableName")]
public class DemoAttributeClassWithCustomTableName : BaseClass
{
    [Column(Name = "CustomColumnName", IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    [OneToMany(TargetEntity = typeof(DemoAttributeAsChildClass), MappedBy = "ParentId")]
    public virtual List<DemoAttributeAsChildClass>? Children { get; set; }
}

[Table]
public class DemoAttributeAsChildClass : BaseClass
{
    [Column]
    public string Description { get; set; } = string.Empty;

    [ForeignKey(ReferenceTable = typeof(DemoAttributeClassWithCustomTableName))]
    public int ParentId { get; set; }
}

public class BaseClass
{
    [Key]
    public int Id { get; set; }
}


[Table]
public class DemoAttributeOneToOneParent
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(ReferenceTable = typeof(DemoAttributeOneToOneChild))]
    public int ChildId { get; set; }

    [OneToOne(TargetEntity = typeof(DemoAttributeOneToOneChild), ForeignKey = "ParentId")]
    public virtual DemoAttributeOneToOneChild? Child { get; set; }
}

[Table]
public class DemoAttributeOneToOneChild : BaseClass
{

}