namespace StudentRunner.Resources;

public interface IRepository<T>
{
    Task<IList<T>> GetAll();

    Task<T?> FindById(int id);

    Task<bool> Create(T entity);

    Task<bool> Update(T entity);

    Task<bool> Delete(int id);
}