using Microsoft.EntityFrameworkCore;
using StudentRunner.Model;

namespace StudentRunner.Resources;

public class Repository<T>(Database db): IDisposable, IRepository<T> where T : BaseEntity
{
    public async Task<IList<T>> GetAll()
    {
        return await db.Set<T>().ToListAsync();
    }

    public async Task<T?> FindById(int id)
    {
        return await db.Set<T>().FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<int> Create(T entity)
    {
        entity.Id = default;
        entity.TimeStamp = DateTime.UtcNow;
        var i = await db.Set<T>().AddAsync(entity);
        await db.SaveChangesAsync();
        return i.Entity.Id;
    }

    public async Task<bool> Update(T entity)
    {
        var oldEntity = await FindById(entity.Id);
        if (oldEntity is null) return false;

        await Create(entity);

        oldEntity.Deleted = true;
        oldEntity.TimeStamp = DateTime.UtcNow;

        db.Set<T>().Update(oldEntity);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete(int id)
    {
        var oldEntity = await FindById(id);
        if (oldEntity is null) return false;

        oldEntity.Deleted = true;
        oldEntity.TimeStamp = DateTime.UtcNow;
        db.Set<T>().Remove(oldEntity);

        return await db.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        db.Dispose();
    }
}