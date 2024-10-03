using KeyStoreApi.Models;

namespace KeyStoreApi.Persistence;

public class KeyStoreRepository : Repository<KeyEntry>
{ 
    public KeyStoreRepository(KeyDbContext db): base(db) 
    {
        
    }
}