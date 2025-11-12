using System.Reflection;
using FhOoeProjectPackages.Database.DbAttributes;

namespace FhOoeProjectPackages.Database.Utilities;

internal class AttributeConverter
{
    public static string GetTableName(Type type)
    {
        var table = type.GetCustomAttribute<TableAttribute>();
        
        return table?.Name ?? type.Name;
    }

    public static string GetTableName<T>()
    {
        return GetTableName(typeof(T));
    }

    public static ICollection<ColumnField> GetFieldInfos<T>()
    {
        return GetFieldInfos(typeof(T));
    }

    public static ICollection<ColumnField> GetFieldInfos(Type type)
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var fields = new List<ColumnField>();

        foreach (var prop in props)
        {
            var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
            var keyAttr = prop.GetCustomAttribute<KeyAttribute>();
            var fkAttr = prop.GetCustomAttribute<ForeignKeyAttribute>();

            var name = columnAttr?.Name ?? keyAttr?.Name ?? fkAttr?.Name ?? prop.Name;
            var isPrim = keyAttr != null || (columnAttr?.IsPrimaryKey ?? false);
            var isForeign = fkAttr != null || (columnAttr?.IsForeignKey ?? false);
            var autoInc = keyAttr?.AutoIncrement ?? columnAttr?.AutoIncrement ?? false;


            var field = new ColumnField(
                FieldName: prop.Name,
                Name: string.IsNullOrWhiteSpace(name) ? prop.Name : name,
                IsPrimaryKey: isPrim,
                IsForeignKey: isForeign,
                IsNullable: columnAttr?.IsNullable ?? true,
                AutoIncrement: autoInc,
                ReferenceTable: fkAttr?.ReferenceTable,
                Prop: prop
            );

            fields.Add(field);
        }

        return fields;
    }
}