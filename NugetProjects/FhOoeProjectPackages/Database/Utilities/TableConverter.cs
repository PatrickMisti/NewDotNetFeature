using System.Reflection;
using FhOoeProjectPackages.Database.DbAttributes;

namespace FhOoeProjectPackages.Database.Utilities;

public class TableConverter
{
    public IList<ColumnDataSet> Columns { get; init; }

    public string TableName { get; init; }

    public Type Type { get; init; }

    public PropertyInfo[] Properties => Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

    private TableConverter(IList<ColumnDataSet> columns, string tableName, Type type)
    {
        Columns = columns;
        TableName = tableName;
        Type = type;
    }

    public static TableConverter GetClassInfo<T>()
    {
        var type = typeof(T);

        if (type is null)
            throw new ArgumentException("The provided class is null.");

        var tableName = GetTableName(type);

        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var columns = GetColumns(props);
        var key = GetKey(props);
        var hasColumnKey = HasKeyInColumn(columns);

        if (key is null && !hasColumnKey || key is not null && hasColumnKey)
            throw new ArgumentException("The provided class must have a Key attribute defined or a Column attribute with IsPrimaryKey set to true.");

        if (key is not null)
            columns.Add(key);

        return new TableConverter(columns, tableName, type);
    }

    public PropertyInfo FindPropertyInfo(ColumnDataSet propertyName) => Properties.First(x => x.Name == propertyName.FieldName);
    
    public PropertyInfo FindKeyPropertyInfo() => Properties.First(x => x.Name == Columns.First(y => y.IsPrim).FieldName);
    
    private static bool HasKeyInColumn(IList<ColumnDataSet> list)
    {
        var key = list.Where(x => x.IsPrim).ToList();

        if (key.Count > 1)
            throw new ArgumentException("The provided class must have only one primary key defined.");

        return key.Any();
    }


    private static ColumnDataSet? GetKey(PropertyInfo[] props)
    {
        var key = props.FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() is not null);

        if (key is null)
            return null;

        var casted = key.GetCustomAttribute<KeyAttribute>();

        return new ColumnDataSet(
            FieldName: key.Name, 
            Name: casted?.Name ?? key.Name, 
            IsPrim: true,
            IsForeign: false,
            AutoIncr: casted?.AutoIncrement ?? true);
    }

    private static IList<ColumnDataSet> GetColumns(PropertyInfo[] props)
    {
        var col = props
            .Where(x => x.GetCustomAttribute<ColumnAttribute>() is not null)
            .Select(x =>
            {
                var cols = x.GetCustomAttribute<ColumnAttribute>();
                return new ColumnDataSet(
                    FieldName: x.Name,
                    Name: cols?.Name ?? x.Name,
                    IsPrim: cols!.IsPrimaryKey,
                    IsForeign: cols.IsForeignKey,
                    AutoIncr: cols.AutoIncrement);
            })
            .ToList();


        if (col is null || !col.Any())
            throw new ArgumentException("The provided class must have at least one Column attribute defined.");

        return col;
    }

    private static string GetTableName(Type type)
    {
        var table = type.GetCustomAttribute<TableAttribute>();
        if (table is null)
            throw new ArgumentException("The provided class does not have a Table attribute.");
        return table.Name ?? type.Name;
    }

    public record ColumnDataSet(string FieldName, string Name, bool IsPrim, bool IsForeign, bool AutoIncr);
}