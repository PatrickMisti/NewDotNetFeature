using System.Reflection;

namespace FhOoeProjectPackages.Database.DbAttributes;

public abstract class BaseAttribute : Attribute
{
    public string? Name { get; set; } = null;
}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : BaseAttribute
{
    public bool IsPrimaryKey { get; set; } = false;
    public bool IsForeignKey { get; set; } = false;
    public bool IsNullable { get; set; } = true;
    public bool AutoIncrement { get; set; } = false;
}

[AttributeUsage(AttributeTargets.Property)]
public class KeyAttribute : BaseAttribute
{
    public bool AutoIncrement { get; set; } = true;
    public bool IsPrimaryKey => true;
}

[AttributeUsage(AttributeTargets.Property)]
public class ForeignKeyAttribute : BaseAttribute
{
    public required Type ReferenceTable { get; set; }
}


public record ColumnField(string FieldName, string Name, bool IsPrimaryKey, bool IsForeignKey, bool IsNullable, bool AutoIncrement, Type? ReferenceTable, PropertyInfo Prop);