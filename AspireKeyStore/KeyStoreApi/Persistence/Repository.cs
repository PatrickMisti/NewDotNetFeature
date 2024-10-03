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
}

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private KeyDbContext _context;

    public Repository(KeyDbContext context)
    {
        _context = context;
        context.InitDb();
    }

    public Task<List<T>> All()
    {
        return _context.Set<T>().ToListAsync();
    }

    public Task<T?> ById(int entityId)
    {
        return _context.Set<T>().FirstOrDefaultAsync(i => i.Id == entityId);
    }

    public async Task<bool> Create(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        var result = await _context.SaveChangesAsync();

        return result > 0;
    }

    public async Task<bool> Update(T entity)
    {
        var db = _context.Set<T>();
        var updateOld = await Delete(entity);

        if (!updateOld)
            return false;

        await db.AddAsync(entity);

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> Delete(T entity)
    {
        var db = _context.Set<T>();
        var resultEntity = await db.FirstOrDefaultAsync(i => i.Id == entity.Id);

        if (resultEntity == null)
            return false;

        resultEntity.Deleted = true;
        db.Update(resultEntity);

        var result = await _context.SaveChangesAsync();

        return result > 0;
    }
}