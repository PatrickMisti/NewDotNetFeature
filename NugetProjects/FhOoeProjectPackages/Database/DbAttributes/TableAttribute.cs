namespace FhOoeProjectPackages.Database.DbAttributes;

[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute : Attribute
{
    public string? Name { get; set; } = null;
}