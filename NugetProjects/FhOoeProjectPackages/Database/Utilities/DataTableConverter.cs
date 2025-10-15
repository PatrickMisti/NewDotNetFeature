using System.Data;
using System.Data.Common;
using System.Globalization;

namespace FhOoeProjectPackages.Database.Utilities;

internal class DataTableConverter
{
    internal static IEnumerable<T> MapRows<T>(DataTable table, TableConverter classInfo) where T : new()
    {
        var list = new List<T>(table.Rows.Count);

        foreach (DataRow row in table.Rows)
        {
            var obj = new T();

            foreach (var col in classInfo.Columns)
            {
                if (!table.Columns.Contains(col.Name)) continue;

                var raw = row[col.Name];
                if (raw == DBNull.Value) continue;

                var prop = classInfo.FindPropertyInfo(col);
                if (!prop.CanWrite) continue;

                var value = ConvertTo(prop.PropertyType, raw);
                var set = prop.GetSetMethod(nonPublic: true);

                if (set is null) continue;
                set.Invoke(obj, [value]);
            }

            list.Add(obj);
        }

        return list;
    }

    private static object? ConvertTo(Type targetType, object raw)
    {
        var t = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (t.IsEnum)
        {
            if (raw is string s) return Enum.Parse(t, s, ignoreCase: true);
            var underlying = Enum.GetUnderlyingType(t);
            var num = Convert.ChangeType(raw, underlying, CultureInfo.InvariantCulture)!;
            return Enum.ToObject(t, num);
        }

        if (t == typeof(Guid))
            return raw is Guid g ? g : Guid.Parse(raw.ToString()!);

        if (t == typeof(DateTimeOffset))
        {
            if (raw is DateTimeOffset dto) return dto;
            if (raw is DateTime dt) return new DateTimeOffset(dt);
            return DateTimeOffset.Parse(raw.ToString()!, CultureInfo.InvariantCulture);
        }

        return Convert.ChangeType(raw, t, CultureInfo.InvariantCulture);
    }

    internal static (string insert, bool withReturn) GetInsertQueryFromProvider(DbConnection con, string table, string columns, string parameters) => con.GetType().FullName switch
    {
        "Microsoft.Data.Sqlite" => ($"INSERT INTO {table} ({columns}) VALUES ({parameters}) RETURNING *;", true),
        "Microsoft.Data.SqlClient" => ($"INSERT INTO {table} ({columns}) OUTPUT INSERTED.* VALUES ({parameters});", true),
        _ => ($"INSERT INTO {table} ({columns}) VALUES ({parameters})", false)
    };
}