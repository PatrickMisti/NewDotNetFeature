using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Reflection;
using System.Text;
using FhOoeProjectPackages.Database.DbAttributes;

namespace FhOoeProjectPackages.Database.Utilities;

public class DatabaseUtils
{
    internal static string GenerateInsertStmt(string tableName, string id, ICollection<string> columns, DbConnection connection)
    {
        var columnNames = string.Join(", ", columns);
        var paramNames = string.Join(", ", columns.Select(c => "@" + c));
        var providerName = connection.GetType().Name;

        var builder = new StringBuilder();

        builder.Append($"INSERT INTO {tableName} ({columnNames})");
        var valuesStmt = $" VALUES ({paramNames})";

        if (providerName.Contains("SqlClient"))
        {
            builder.Append($" OUTPUT INSERTED.{id}");
            builder.Append($" {valuesStmt};");
        }
        else if (providerName.Contains("Npgsql") || providerName.Contains("Sqlite"))
        {
            builder.Append($" {valuesStmt}");
            builder.Append($" RETURNING {id};");
        }

        return builder.ToString();
    }

    internal static string GenerateUpdateStmt(string tableName, string id, ICollection<string> columns)
    {
        var setClauses = string.Join(", ", columns.Select(c => $"{c} = @{c}"));
        var builder = new StringBuilder();
        builder.Append($"UPDATE {tableName} SET {setClauses} WHERE {id} = @{id};");
        return builder.ToString();
    }

    public static string GenerateTableStmt<T>(string providerName)
    {
        var tableName = AttributeConverter.GetTableName<T>();
        var fields = AttributeConverter.GetFieldInfos<T>();
        var builder = new StringBuilder();

        var prim = fields.First(x => x.IsPrimaryKey);
        var foreign = fields.FirstOrDefault(x => x.IsForeignKey);

        var columns = fields
            .Where(x => x.Prop.GetCustomAttribute<ColumnAttribute>() is not null &&
                        !ReferenceEquals(x, prim) && !ReferenceEquals(x, foreign));

        builder.Append($"CREATE TABLE {tableName} (");
        builder.Append(GetIdDefinition(providerName, prim.Name, prim.AutoIncrement));
        builder.Append(", ");

        foreach (var column in columns)
        {
            builder.Append($"{column.Name} {GetSqlTypeForProvider(column.Prop.PropertyType, providerName)}");
            if (!column.IsNullable)
                builder.Append(" NOT NULL");
            builder.Append(", ");
        }

        if (foreign != null && foreign.ReferenceTable is not null)
        {
            builder.Append($"{foreign.Name} {GetSqlTypeForProvider(foreign.Prop.PropertyType, providerName)}, ");

            var foreignTableName = AttributeConverter.GetTableName(foreign.ReferenceTable);
            var foreignKey = AttributeConverter.GetFieldInfos(foreign.ReferenceTable)
                .FirstOrDefault(x => x.IsPrimaryKey)
                ?? throw new ArgumentException("Class has no id");

            builder.Append($"FOREIGN KEY ({foreign.Name}) REFERENCES {foreignTableName}({foreignKey.Name}), ");
        }

        builder.Length -= 2;
        builder.Append(");");

        return builder.ToString();
    }

    private static string GetSqlTypeForProvider(Type t, string providerName)
    {
        var p = providerName.ToLower();

        if (t == typeof(int))
            return p.Contains("sqlite") ? "INTEGER" : "INT"; // wichtig für SQLite

        if (t == typeof(string))
            return "VARCHAR(255)";
        if (t == typeof(DateTime))
            return p.Contains("sqlite") ? "TEXT" : "DATETIME"; // SQLite speichert Dates oft als TEXT/NUMERIC
        if (t == typeof(bool))
            return p.Contains("sqlite") ? "INTEGER" : "BIT";   // SQLite hat kein BIT; 0/1 in INTEGER
        if (t == typeof(double))
            return p.Contains("sqlite") ? "REAL" : "FLOAT";

        throw new NotSupportedException($"The property type {t.Name} is not supported.");
    }

    private static string GetIdDefinition(string providerName, string columnName, bool autoIncrement)
    {
        var p = providerName.ToLower();

        if (!autoIncrement)
        {
            var baseType = p.Contains("sqlite") ? "INTEGER" : "INT";
            return $"{columnName} {baseType} PRIMARY KEY";
        }

        if (p.Contains("sqlite"))
            return $"{columnName} INTEGER PRIMARY KEY AUTOINCREMENT";

        if (p.Contains("sqlclient"))
            return $"{columnName} INT IDENTITY(1,1) PRIMARY KEY";

        if (p.Contains("mysql"))
            return $"{columnName} INT AUTO_INCREMENT PRIMARY KEY";

        if (p.Contains("npgsql") || p.Contains("postgres"))
            return $"{columnName} INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY";

        if (p.Contains("oracle"))
            return $"{columnName} NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY";

        return $"{columnName} INT PRIMARY KEY";
    }
}

public static class DbProviderRegistration
{
    private static bool _sqliteRegistered;

    public static void EnsureSqliteRegistered()
    {
        if (_sqliteRegistered) return;

        DbProviderFactories.RegisterFactory(
            "Microsoft.Data.Sqlite",
            SqliteFactory.Instance
        );

        _sqliteRegistered = true;
    }
}