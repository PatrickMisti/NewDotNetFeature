using KeyStoreApi.Models;
using KeyStoreApi.Persistence;

namespace KeyStoreApi.Services;

public interface IKeyStoreService
{
    Task<List<KeyEntry>> GetAsync();

    Task<bool> AddKey(KeyEntry key);
}

public class KeyStoreService : IKeyStoreService
{
    private KeyStoreRepository _repository;

    public KeyStoreService(KeyStoreRepository repo)
    {
        _repository = repo;
    }

    public async Task<List<KeyEntry>> GetAsync()
    {
        try
        {
            return await _repository.All();
        }
        catch 
        {
            return new List<KeyEntry>();
        }
    }

    public async Task<bool> AddKey(KeyEntry key)
    {
        try
        {
            var result = await _repository.Create(key);
            if (!result)
                throw new Exception("Could not save!");

            return true;
        }
        catch 
        {
            return false;
        }
    }
}