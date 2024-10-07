using KeyStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyStoreApi.Persistence;

interface IRepository<T>
{
    Task<List<T>> All();
    Task<T?> ById(int entityId);
    Task<bool> Create(T entity);
    Task<bool> Update(T entity);
    Task<bool> Delete(T entity);
    Task<bool> DeleteById(int id);
}

public class Repository<T>(KeyDbContext context) : IRepository<T>
    where T : BaseEntity
{
    public Task<List<T>> All()
    {
        return context.Set<T>().ToListAsync();
    }

    public Task<T?> ById(int entityId)
    {
        return context.Set<T>().FirstOrDefaultAsync(i => i.Id == entityId);
    }

    public async Task<bool> Create(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        var result = await context.SaveChangesAsync();

        return result > 0;
    }

    public async Task<bool> Update(T entity)
    {
        var db = context.Set<T>();
        var updateOld = await Delete(entity);

        if (!updateOld)
            return false;

        await db.AddAsync(entity);

        var result = await context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> Delete(T entity)
    {
        var db = context.Set<T>();
        var resultEntity = await db.FirstOrDefaultAsync(i => i.Id == entity.Id);

        if (resultEntity == null)
            return false;

        resultEntity.Deleted = true;
        db.Update(resultEntity);

        var result = await context.SaveChangesAsync();

        return result > 0;
    }

    public async Task<bool> DeleteById(int id)
    {
        var db = context.Set<T>();
        var result = await db.FirstOrDefaultAsync(i => i.Id == id);

        if (result == null) return false;

        return await Delete(result);
    }
}