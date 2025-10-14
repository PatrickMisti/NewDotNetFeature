using System.Reflection;
using FhOoeProjectPackages.Database.DbAttributes;

namespace FhOoeProjectPackages.Database.Utlities;

internal class TableConverter<T>
{
    public IList<ColumnAttribute> Columns { get; init; }

    public ColumnAttribute Key { get; init; }
    public string TableName { get; init; }

    public TableConverter(T element)
    {
        if (element is null)
            throw new ArgumentNullException(nameof(element), "The provided element cannot be null.");

        var type = element.GetType();
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        TableName = GetName(type);
        var key = GetKey(props);
        var col = GetColumns(props);
        var hasKeyInCol = col.FirstOrDefault(x => x.IsPrimaryKey);

        if ((key is null && hasKeyInCol is null) || (key is not null && hasKeyInCol is not null))
            throw new ArgumentException("The provided class must have exactly one primary key defined, either via KeyAttribute or ColumnAttribute with IsPrimaryKey set to true.");

        Columns = col.Where(x => !x.IsPrimaryKey).ToList();
        Key = new ColumnAttribute
        {
            Name = key?.Name ?? hasKeyInCol?.Name ?? throw new ArgumentException("No name found for key!!"),
            IsPrimaryKey = true,
            AutoIncrement = key?.AutoIncrement ?? hasKeyInCol?.AutoIncrement ?? false,
            IsForeignKey = false,
            IsNullable = false
        };
    }

    private static string GetName(Type element)
    {
        var name = element.GetCustomAttribute<TableAttribute>()?.Name;
        return name ?? element.Name;
    }

    private static KeyAttribute? GetKey(PropertyInfo[] props)
    {
        var key = props.FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() is not null);

        if (key is null)
            return null;

        var casted = key.GetCustomAttribute<KeyAttribute>();

        return new KeyAttribute()
        {
            Name = casted?.Name ?? key.Name,
            AutoIncrement = casted!.AutoIncrement
        };
    }

    private static IList<ColumnAttribute> GetColumns(PropertyInfo[] props)
    {
        var col = props
            .Where(x => x.GetCustomAttributes<ColumnAttribute>() is not null)
            .SelectMany(x => x.GetCustomAttributes<ColumnAttribute>());

        if (col is null)
            throw new ArgumentException("The provided class must have at least one Column attribute defined.");
        return col.ToList();
    }

    public static string GetTableName<T>() where T : class, new()
    {
        var type = typeof(T);
        var table = type.GetCustomAttribute<TableAttribute>();
        if (table is null)
            throw new ArgumentException("The provided class does not have a Table attribute.");
        return table.Name ?? type.Name;
    }
}