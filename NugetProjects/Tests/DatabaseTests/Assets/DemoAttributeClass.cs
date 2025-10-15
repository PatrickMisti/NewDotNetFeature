using FhOoeProjectPackages.Database.DbAttributes;

namespace Tests.DatabaseTests.Assets;

[Table]
public class DemoAttributeClass()
{
    [Key(Name = "DemoKey")]
    public int Id { get; set; }

    [Column(Name = "DemoColumn")]
    public string Name { get; set; }

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