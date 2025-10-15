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

    public static string CreateTableFromClass(TableConverter classInfo)
    {
        var cols = classInfo.Columns.Select(c =>
        {
            var prop = classInfo.FindPropertyInfo(c);
            var type = prop.PropertyType;
            var isNullable = Nullable.GetUnderlyingType(type) != null || !type.IsValueType;

            var sqlType = ToSqlType(type);
            var nullStr = isNullable || c.IsNullable ? "NULL" : "NOT NULL";

            var parts = new List<string>
            {
                c.Name,
                sqlType,
                nullStr
            };
            if (c.IsPrim) parts.Add("PRIMARY KEY");
            if (c.AutoIncr) parts.Add("AUTOINCREMENT");

            // No ForeignKeyInfo available for now (need more attributes), so skipping FOREIGN KEY constraint

            return string.Join(" ", parts);
        });

        var columnsDef = string.Join(", ", cols);
        return $"CREATE TABLE IF NOT EXISTS {classInfo.TableName} ({columnsDef});";
    }

    private static string ToSqlType(Type type)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;

        return t switch
        {
            _ when t.IsEnum => "INTEGER",
            var tt when tt == typeof(string) => "TEXT",
            var tt when tt == typeof(int) || tt == typeof(long) || tt == typeof(short) || tt == typeof(byte) => "INTEGER",
            var tt when tt == typeof(bool) => "BOOLEAN",
            var tt when tt == typeof(DateTime) || tt == typeof(DateTimeOffset) => "DATETIME",
            var tt when tt == typeof(float) || tt == typeof(double) || tt == typeof(decimal) => "REAL",
            var tt when tt == typeof(Guid) => "TEXT",
            _ => "BLOB"
        };
    }

    public static void EnsureTableExists(DbConnection connection, string sql)
    {
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
        connection.Close();
    }
}