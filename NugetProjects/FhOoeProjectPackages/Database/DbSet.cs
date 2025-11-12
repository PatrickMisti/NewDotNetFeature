using System.Collections;
using FhOoeProjectPackages.Database.DbAttributes;
using FhOoeProjectPackages.Database.Utilities;
using System.Data;
using System.Data.Common;
using System.Reflection;

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

    private static object? FillInstanceFromDb(Type type, DbDataReader read, IEnumerable<ColumnField> fieldInfos, Func<PropertyInfo,bool> checkAttribute)
    {
        var instance = Activator.CreateInstance(type);

        foreach (var col in fieldInfos)
        {
            if (!col.Prop.CanWrite || !checkAttribute.Invoke(col.Prop)) continue;

            var value = read[col.Name];

            if (value is DBNull)
            {
                if (Nullable.GetUnderlyingType(col.Prop.PropertyType) != null)
                    col.Prop.SetValue(instance, null);
                continue;
            }

            col.Prop.SetValue(
                instance,
                DbValueConverter.Convert(value, col.Prop.PropertyType));
        }

        // Todo Load One.To.One relations
        // GetByIdAsync should be used to load the relations

        return instance;
    }

    private bool HasAttribute(PropertyInfo info) => info.GetCustomAttribute<KeyAttribute>() is not null ||
                                                  info.GetCustomAttribute<ColumnAttribute>() is not null ||
                                                  info.GetCustomAttribute<ForeignKeyAttribute>() is not null;

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

    private async Task EnsureOpenAsync(CancellationToken ct)
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync(ct);
    }

    public async Task<IEnumerable<T>> GetAllAsync(bool enableEagerLoading = false, CancellationToken token = default)
    {
        var sqlStmt = $"SELECT * from {_tableName}";

        await EnsureOpenAsync(token);

        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;
        await using var read = await command.ExecuteReaderAsync(token);

        var results = new List<T>();
        while (await read.ReadAsync(token))
        {
            var item = (T?)FillInstanceFromDb(typeof(T), read, _fieldInfos, HasAttribute);
            if (item is null) continue;
            results.Add(item);
        }
        
        if (enableEagerLoading)
            await EagerLoadRelationsAsync(results, token);
        return results;
    }

    public async Task<int?> AddAsync(T element, CancellationToken token = default)
    {
        var nonAutoIncList = _fieldInfos
            .Where(x => !x.AutoIncrement && x.Prop.GetCustomAttribute<BaseAttribute>() is not null)
            .ToList();

        var sqlStmt = DatabaseUtils.GenerateInsertStmt(
            tableName: _tableName,
            id: _fieldInfos.Single(x => x.IsPrimaryKey).Name,
            columns: nonAutoIncList.Select(x => x.Name).ToList(),
            connection: _connection);


        await EnsureOpenAsync(token);

        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;

        UpdateEntitySqlStmtToCommand(command, nonAutoIncList, element);

        var result = await command.ExecuteScalarAsync(token);
        if (result is null) return null;
        return Convert.ToInt32(result);
    }

    public async Task<T?> GetByIdAsync(int id, bool enableEagerLoading = false, CancellationToken token = default)
    {
        var keyColumn = _fieldInfos.First(x => x.IsPrimaryKey);
        var sqlStmt = $"SELECT * from {_tableName} where {keyColumn.Name} = @{keyColumn.Name}";

        await EnsureOpenAsync(token);
        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;

        UpdateSqlStmtToCommand(command, keyColumn.Name, id);

        await using var reader = await command.ExecuteReaderAsync(token);

        if (!await reader.ReadAsync(token)) return null;

        var result = (T?)FillInstanceFromDb(typeof(T), reader, _fieldInfos, HasAttribute);
        if (result is null) return null;

        if (enableEagerLoading)
            await EagerLoadRelationsAsync([result], token);
        return result;
    }

    public async Task<bool> UpdateAsync(T element, CancellationToken token = default)
    {
        var keyCol = _fieldInfos.First(c => c.IsPrimaryKey);

        var nonPrimFields = _fieldInfos.Where(c => !c.IsPrimaryKey && HasAttribute(c.Prop)).ToList();
        if (nonPrimFields.Count == 0) return false;

        var sqlStmt = DatabaseUtils.GenerateUpdateStmt(
            tableName: _tableName,
            id: keyCol.Name,
            columns: nonPrimFields.Select(c => c.Name).ToList());

        await EnsureOpenAsync(token);
        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;

        UpdateEntitySqlStmtToCommand(command,[..nonPrimFields, keyCol], element);
        int affected = await command.ExecuteNonQueryAsync(token);

        return affected > 0;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        var keyCol = _fieldInfos.First(c => c.IsPrimaryKey);
        var sqlStmt = $"DELETE FROM {_tableName} WHERE {keyCol.Name} = @{keyCol.Name}";

        await EnsureOpenAsync(token);
        await using var command = _connection.CreateCommand();
        command.CommandText = sqlStmt;

        UpdateSqlStmtToCommand(command, keyCol.Name, id);

        var affected = await command.ExecuteNonQueryAsync(token);

        return affected > 0;
    }

    private async Task EagerLoadRelationsAsync(IEnumerable<T> parentsList, CancellationToken token)
    {
        var relations = DatabaseUtils.GetOneToManyRelations(typeof(T));
        var parents = parentsList as IList<T> ?? parentsList.ToList();
        if (parents.Count == 0 || !relations.Any()) return;

        var parentKey = _fieldInfos.First(f => f.IsPrimaryKey);
        var parentKeyProp = parentKey.Prop;

        var allParentIds = parents
            .Select(p => Convert.ToInt32(parentKeyProp.GetValue(p)!))
            .Distinct()
            .ToList();
        if (allParentIds.Count == 0) return;

        foreach (var (navProp, meta) in relations)
        {
            var childType = meta.TargetEntity;
            var fkName = meta.MappedBy;
            var childTable = AttributeConverter.GetTableName(childType);
            var childFields = AttributeConverter.GetFieldInfos(childType);

            await EnsureOpenAsync(token);
            var command = _connection.CreateCommand();

            for (int i = 0; i < allParentIds.Count; i++)
                UpdateSqlStmtToCommand(command,$"p{i}", allParentIds[i]);
            

            var paramNames = Enumerable.Range(0, allParentIds.Count)
                .Select(i => $"@p{i}")
                .ToList();

            command.CommandText = $"SELECT * FROM {childTable} WHERE {fkName} IN ({string.Join(",", paramNames)})";

            var childrenByParent = new Dictionary<int, IList<object>>();
            await using var reader = await command.ExecuteReaderAsync(token);

            while (await reader.ReadAsync(token))
            {
                var child = FillInstanceFromDb(childType, reader, childFields, HasAttribute);

                var fkVal = (int)Convert.ChangeType(reader[fkName], typeof(int))!;
                if (!childrenByParent.TryGetValue(fkVal, out var list))
                    list = childrenByParent[fkVal] = new List<object>();
                if (child is null) continue;
                list.Add(child);
            }

            foreach (var parent in parents)
            {
                var id = Convert.ToInt32(parentKeyProp.GetValue(parent)!);
                var listType = typeof(List<>).MakeGenericType(childType);
                var list = (IList)Activator.CreateInstance(listType)!;

                if (childrenByParent.TryGetValue(id, out var kids))
                    foreach (var k in kids) 
                        list.Add(k);

                navProp.SetValue(parent, list);
            }
        }
    }

}