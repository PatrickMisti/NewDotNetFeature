using FhOoeProjectPackages.Database.Utilities;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace FhOoeProjectPackages.Database;

public class DbSet<T>(DbConnection connection) where T : class, new()
{
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var classInfo = TableConverter.GetClassInfo<T>();
        var sqlStmt = $"SELECT * from {classInfo.TableName}";

        var table = new DataTable();

        try
        {
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sqlStmt;
            command.CommandType = CommandType.Text;
            await using var read = await command.ExecuteReaderAsync();

            table.Load(read);
        }
        finally
        {
            if (connection.State != ConnectionState.Closed)
                await connection.CloseAsync();
        }

        return MapRows(table, classInfo);
    }

    public async Task<bool> AddAsync(T element)
    {
        var classInfo = TableConverter.GetClassInfo<T>();
        var insertCols = classInfo.Columns.Where(c => !c.AutoIncr).ToList();

        var columnNames = string.Join(", ", insertCols.Select(c => c.Name));
        var paramNames = string.Join(", ", insertCols.Select(c => "@" + c.Name));

        var sqlStmt = $"INSERT INTO {classInfo.TableName} ({columnNames}) VALUES ({paramNames})";

        try
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = sqlStmt;
            command.CommandType = CommandType.Text;

            foreach (var col in insertCols)
            {
                var prop = classInfo.FindPropertyInfo(col);
                var value = prop.GetValue(element) ?? DBNull.Value;

                var param = command.CreateParameter();
                param.ParameterName = "@" + col.Name;
                param.Value = value;
                command.Parameters.Add(param);
            }
            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }
        finally
        {
            if (connection.State != ConnectionState.Closed)
                await connection.CloseAsync();
        }
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var classInfo = TableConverter.GetClassInfo<T>();
        var keyColumn = classInfo.Columns.First(x => x.IsPrim);
        var sqlStmt = $"SELECT * from {classInfo.TableName} where {keyColumn.Name} = @Id";
        var table = new DataTable();
        try
        {
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sqlStmt;
            command.CommandType = CommandType.Text;

            var p = command.CreateParameter();
            p.ParameterName = "@Id";
            p.Value = id;
            command.Parameters.Add(p);

            await using var reader = await command.ExecuteReaderAsync();
            table.Load(reader);
        }
        finally
        {
            if (connection.State != ConnectionState.Closed)
                await connection.CloseAsync();
        }

        return MapRows(table, classInfo).FirstOrDefault();
    }

    /*

    public bool Update<T>(T element) where T : class, new()
    {
        var command = connection.CreateCommand();
        var converter = new TableConverter<T>(element);
        var keyColumn = converter.Columns.FirstOrDefault(c => c.IsPrimaryKey);

        if (keyColumn == null)
            throw new ArgumentException("The provided class must have a primary key defined.");

        var setClauses = string
            .Join(", ", converter.Columns
                .Where(c => !c.IsPrimaryKey)
                .Select(c => $"{c.Name} = @{c.Name}"));

        command.CommandText = $"UPDATE {converter.TableName} SET {setClauses} WHERE {keyColumn.Name} = @{keyColumn.Name}";
        
        foreach (var column in converter.Columns)
        {
            var prop = typeof(T).GetProperty(column.Name!);
            var value = prop?.GetValue(element) ?? DBNull.Value;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@" + column.Name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        connection.Open();
        var result = command.ExecuteNonQuery();
        connection.Close();
        return result > 0;
    }

    public bool Delete<T>(T element) where T : class, new()
    {
        var command = connection.CreateCommand();
        var converter = new TableConverter<T>(element);
        var keyColumn = converter.Columns.FirstOrDefault(c => c.IsPrimaryKey);

        if (keyColumn == null)
            throw new ArgumentException("The provided class must have a primary key defined.");

        command.CommandText = $"DELETE FROM {converter.TableName} WHERE {keyColumn.Name} = @{keyColumn.Name}";

        var prop = typeof(T).GetProperty(keyColumn.Name!);
        var value = prop?.GetValue(element) ?? DBNull.Value;
        var parameter = command.CreateParameter();

        parameter.ParameterName = "@" + keyColumn.Name;
        parameter.Value = value;
        command.Parameters.Add(parameter);

        connection.Open();
        var result = command.ExecuteNonQuery();
        connection.Close();
        return result > 0;
    }*/

    private static IEnumerable<T> MapRows(DataTable table, TableConverter classInfo)
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
}