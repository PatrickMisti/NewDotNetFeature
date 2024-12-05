namespace Connection.Services;

public interface IRepository<T>
{
    Task<IList<T>> GetAllAsync();

    Task<T?> GetById(int id);

    Task<T> Create(T entity);

    Task<bool> Update(T entity);

    Task<bool> Delete(int id);
}