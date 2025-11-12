using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Text;

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

    public static string GenerateTableStmt<T>()
    {
        var tableName = AttributeConverter.GetTableName<T>();
        var fields = AttributeConverter.GetFieldInfos<T>();
        var builder = new StringBuilder();

        var prim = fields.First(x => x.IsPrimaryKey);
        var foreign = fields.FirstOrDefault(x => x.IsForeignKey);
        var columns = fields.Except([prim, foreign]);

        builder.Append($"CREATE TABLE {tableName} (");
        builder.Append($"{prim.Name} INT");
        if (prim.AutoIncrement)
            builder.Append(" IDENTITY(1,1)");
        builder.Append(" PRIMARY KEY, ");

        foreach (var column in columns)
        {
            builder.Append($"{column.Name} {GetSqlType(column.Prop.PropertyType)}");
            if (!column.IsNullable)
                builder.Append(" NOT NULL");
            builder.Append(", ");
        }

        if (foreign != null)
        {
            builder.Append($"{foreign.Name} {GetSqlType(foreign.Prop.PropertyType)}, ");
            builder.Append($"FOREIGN KEY ({foreign.Name}) REFERENCES {foreign.ReferenceTable}({foreign.Name}), ");
        }

        // Remove last comma and space
        builder.Length -= 2;
        builder.Append(");");
        return builder.ToString();
    }

    private static string GetSqlType(Type propPropertyType)
    {
        if (typeof(int) == propPropertyType)
            return "INTEGER";
        if (typeof(string) == propPropertyType)
            return "VARCHAR(255)";
        if (typeof(DateTime) == propPropertyType)
            return "DATETIME";
        if (typeof(bool) == propPropertyType)
            return "BIT";
        if (typeof(double) == propPropertyType)
            return "FLOAT";
        throw new NotSupportedException($"The property type {propPropertyType.Name} is not supported.");
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