using FhOoeProjectPackages.Database.Utilities;
using System.Data;
using System.Data.Common;
using FhOoeProjectPackages.Database.DbAttributes;

namespace FhOoeProjectPackages.Database;

public class DbSet<T> where T : class
{
    private readonly DbConnection _connection;
    private readonly string _tableName;
    private readonly ICollection<ColumnField> _fieldInfos;

    public DbSet(DbConnection connection)
    {
        _connection = connection;
        _tableName = AttributeConverter.GetTableName<T>();
        _fieldInfos = AttributeConverter.GetFieldInfos<T>();
    }

    private T FillInstanceFromDb(DbDataReader read)
    {
        var instance = Activator.CreateInstance<T>();

        foreach (var col in _fieldInfos)
        {
            if (!col.Prop.CanWrite) continue;

            var value = read[col.Name];
            col.Prop.SetValue(
                instance, 
                value == DBNull.Value 
                    ? null 
                    : Convert.ChangeType(value, col.Prop.PropertyType));
        }

        return instance;
    }

    private void UpdateSqlStmtToCommand(DbCommand command, string fieldName, object value)
    {
        var p = command.CreateParameter();
        p.ParameterName = $"@{fieldName}";
        p.Value = value;
        command.Parameters.Add(p);
    }

    private void UpdateEntitySqlStmtToCommand(DbCommand command, ICollection<ColumnField> fields, T element)
    {
        foreach (var field in fields)
        {
            var value = field.Prop.GetValue(element) ?? DBNull.Value;
            UpdateSqlStmtToCommand(command, field.Name, value);
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
    {
        var sqlStmt = $"SELECT * from {_tableName}";

        await _connection.OpenAsync(token);

        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;
        await using var read = await command.ExecuteReaderAsync(token);

        var results = new List<T>();

        while (await read.ReadAsync(token))
            results.Add(FillInstanceFromDb(read));
        
        return results;
    }

    public async Task<int?> AddAsync(T element, CancellationToken token = default)
    {
        var nonAutoIncList = _fieldInfos.Where(x => !x.AutoIncrement).ToList();
        var sqlStmt = DatabaseUtils.GenerateInsertStmt(
            tableName: _tableName,
            id: _fieldInfos.Single(x => x.IsPrimaryKey).Name,
            columns: nonAutoIncList.Select(x => x.Name).ToList(),
            connection: _connection);


        await _connection.OpenAsync(token);
        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;

        UpdateEntitySqlStmtToCommand(command, nonAutoIncList, element);

        return (int?)await command.ExecuteScalarAsync(token);
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var keyColumn = _fieldInfos.First(x => x.IsPrimaryKey);
        var sqlStmt = $"SELECT * from {_tableName} where {keyColumn.Name} = @{keyColumn.Name}";

        await _connection.OpenAsync(token);
        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;

        UpdateSqlStmtToCommand(command, keyColumn.Name, id);

        await using var reader = await command.ExecuteReaderAsync(token);

        if (!await reader.ReadAsync(token)) return null;

        return FillInstanceFromDb(reader);
    }

    public async Task<bool> UpdateAsync(T element, CancellationToken token = default)
    {
        var keyCol = _fieldInfos.First(c => c.IsPrimaryKey);

        var nonPrimFields = _fieldInfos.Where(c => !c.IsPrimaryKey).ToList();
        if (nonPrimFields.Count == 0) return false;

        var sqlStmt = DatabaseUtils.GenerateUpdateStmt(
            tableName: _tableName,
            id: keyCol.Name,
            columns: nonPrimFields.Select(c => c.Name).ToList());

        await _connection.OpenAsync(token);
        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;

        UpdateEntitySqlStmtToCommand(command, nonPrimFields, element);
        int affected = await command.ExecuteNonQueryAsync(token);

        return affected > 0;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        var keyCol = _fieldInfos.First(c => c.IsPrimaryKey);
        var sqlStmt = $"DELETE FROM {_tableName} WHERE {keyCol.Name} = @{keyCol.Name}";

        await _connection.OpenAsync(token);
        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;

        UpdateSqlStmtToCommand(command, keyCol.Name, id);

        var affected = await command.ExecuteNonQueryAsync(token);

        return affected > 0;
    }
}