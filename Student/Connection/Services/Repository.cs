using Connection.Models;
using Microsoft.EntityFrameworkCore;

namespace Connection.Services;

public class Repository<T>: IRepository<T> where T : BaseEntity
{
    protected Database _repo;

    public Repository(Database db)
    {
        _repo = db;
    }

    public async Task<IList<T>> GetAllAsync()
    {
        return await _repo.Set<T>().ToListAsync();
    }

    public async Task<T?> GetById(int id)
    {
        return await _repo.Set<T>().FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<T> Create(T entity)
    {
        var item = await _repo.Set<T>().AddAsync(entity);

        if (await _repo.SaveChangesAsync() <= 0)
            throw new ArgumentException("Could not save entity: " + typeof(T).Name);

        return item.Entity;
    }

    public async Task<bool> Update(T entity)
    {
        var oldEntity = await GetById(entity.Id);
        if (oldEntity is null) return false;

        entity.Id = default;
        await _repo.Set<T>().AddAsync(entity);

        oldEntity.Deleted = true;
        oldEntity.TimeStamp = DateTime.UtcNow;

        _repo.Set<T>().Update(oldEntity);
        return await _repo.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await GetById(id);
        if (entity is null) return false;

        entity.Deleted = true;
        entity.TimeStamp = DateTime.UtcNow;

        _repo.Set<T>().Update(entity);
        return await _repo.SaveChangesAsync() > 0;
    }
}