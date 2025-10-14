namespace FhOoeProjectPackages.Database.DbAttributes;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    public string? Name { get; set; } = null;
    public bool IsPrimaryKey { get; set; } = false;
    public bool IsForeignKey { get; set; } = false;
    public bool IsNullable { get; set; } = true;
    public bool AutoIncrement { get; set; } = false;
}

[AttributeUsage(AttributeTargets.Property)]
public class KeyAttribute : Attribute
{
    public string? Name { get; set; }
    public bool AutoIncrement { get; set; } = true;
}