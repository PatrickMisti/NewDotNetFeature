using FhOoeProjectPackages.Database.Utilities;
using System.Data;
using System.Data.Common;

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

        return DataTableConverter.MapRows<T>(table, classInfo);
    }

    public async Task<T?> AddAsync(T element)
    {
        var classInfo = TableConverter.GetClassInfo<T>();
        var insertCols = classInfo.Columns.Where(c => !c.AutoIncr).ToList();

        var columnNames = string.Join(", ", insertCols.Select(c => c.Name));
        var paramNames = string.Join(", ", insertCols.Select(c => "@" + c.Name));


        var (sqlStmt, withReturn) = DataTableConverter.GetInsertQueryFromProvider(connection, classInfo.TableName, columnNames, paramNames);
        var table = new DataTable();

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

            if (withReturn)
            {
                await using var result = await command.ExecuteReaderAsync();
                table.Load(result);
                return DataTableConverter.MapRows<T>(table, classInfo).FirstOrDefault();
            }
            var affected = await command.ExecuteNonQueryAsync();
            return affected > 0 ? element : null;
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

        return DataTableConverter.MapRows<T>(table, classInfo).FirstOrDefault();
    }

    public async Task<T?> UpdateAsync(T element)
    {
        var classInfo = TableConverter.GetClassInfo<T>();

        var keyCol = classInfo.Columns.First(c => c.IsPrim);
        var keyProp = classInfo.FindKeyPropertyInfo();
        var keyValue = keyProp.GetValue(element) ?? throw new InvalidOperationException("Primary key value is null.");

        var updatableCols = classInfo.Columns.Where(c => !c.IsPrim).ToList();
        if (updatableCols.Count == 0) return null;

        var setClause = string.Join(", ", updatableCols.Select(c => $"{c.Name} = @{c.Name}"));
        var sqlStmt = $"UPDATE {classInfo.TableName} SET {setClause} Where {keyCol.Name} = @Key";

        try
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = sqlStmt;
            command.CommandType = CommandType.Text;

            foreach (var col in updatableCols)
            {
                var prop = classInfo.FindPropertyInfo(col);
                var value = prop.GetValue(element) ?? DBNull.Value;

                var p = command.CreateParameter();
                p.ParameterName = "@" + col.Name;
                p.Value = value;
                command.Parameters.Add(p);
            }

            var keyParam = command.CreateParameter();
            keyParam.ParameterName = "@Key";
            keyParam.Value = keyValue;
            command.Parameters.Add(keyParam);
            var affected = await command.ExecuteNonQueryAsync();

            return affected > 0 ? element : null;
        }
        finally
        {
            if (connection.State != ConnectionState.Closed)
                await connection.CloseAsync();
        }
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var classInfo = TableConverter.GetClassInfo<T>();
        var keyCol = classInfo.Columns.First(c => c.IsPrim);
        var sqlStmt = $"DELETE FROM {classInfo.TableName} WHERE {keyCol.Name} = @Id";

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
            var affected = await command.ExecuteNonQueryAsync();

            return affected > 0;
        }
        finally
        {
            if (connection.State != ConnectionState.Closed)
                await connection.CloseAsync();
        }
    }
}