using System.Data;
using System.Reflection;
using FhOoeProjectPackages.Database.Utlities;

namespace FhOoeProjectPackages.Database;

public class DbSet<T>(IDbConnection connection)
{
    public IEnumerable<T> GetAll<T>() where T : class, new()
    {
        var command = connection.CreateCommand();
        var converter = new TableConverter<T>(new T());
        command.CommandText = $"SELECT * FROM {converter.TableName}";
        connection.Open();

        using var reader = command.ExecuteReader();
        var results = new List<T>();

        while (reader.Read())
        {
            var item = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var prop = typeof(T).GetProperty(reader.GetName(i));
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(item, reader.GetValue(i));
                }
            }
            results.Add(item);
        }

        connection.Close();
        return results;
    }

    public T? GetById<T>(int id) where T : class, new()
    {
        var command = connection.CreateCommand();
        var converter = new TableConverter<T>(new T());

        var keyColumn = converter.Columns.FirstOrDefault(c => c.IsPrimaryKey);
        if (keyColumn == null)
            throw new ArgumentException("The provided class must have a primary key defined.");

        command.CommandText = $"SELECT * FROM {converter.TableName} WHERE {keyColumn.Name} = @Id";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@Id";
        parameter.Value = id;
        command.Parameters.Add(parameter);

        connection.Open();
        using var reader = command.ExecuteReader();

        T? result = null;

        if (reader.Read())
        {
            result = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var prop = typeof(T).GetProperty(reader.GetName(i));
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(result, reader.GetValue(i));
                }
            }
        }

        connection.Close();
        return result;
    }

    public bool Add<T>(T element) where T : class
    {
        var command = connection.CreateCommand();
        var converter = new TableConverter<T>(element);

        var columnNames = string.Join(", ", converter.Columns.Where(c => !c.AutoIncrement).Select(c => c.Name));
        var paramNames = string.Join(", ", converter.Columns.Where(c => !c.AutoIncrement).Select(c => "@" + c.Name));
        
        command.CommandText = $"INSERT INTO {converter.TableName} ({columnNames}) VALUES ({paramNames})";

        foreach (var column in converter.Columns.Where(c => !c.AutoIncrement))
        {
            
            var prop = element.GetType().GetProperty(column.Name!);
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
    }
}