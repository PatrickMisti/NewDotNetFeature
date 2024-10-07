using KeyStoreApi.Models;
using KeyStoreApi.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KeyStoreApi.Services;

public interface IKeyStoreService
{
    Task<List<KeyEntry>> GetAsync();
    Task<bool> AddKeyAsync(KeyEntry key);
    Task<bool> UpdateKeyAsync(KeyEntry newKey);
    Task<bool> DeleteKeyAsync(KeyEntry key);
    Task<bool> DeleteKeyByIdAsync(int id);
}

public class KeyStoreService(KeyStoreRepository repo, ILogger logger) : IKeyStoreService
{
    public async Task<List<KeyEntry>> GetAsync()
    {
        try
        {
            logger.LogDebug("Grab all keys");
            return await repo.All();
        }
        catch (Exception ex) 
        {
            logger.LogError("Could not grab key entries :" + ex.Message);
            return [];
        }
    }

    public async Task<bool> AddKeyAsync(KeyEntry key)
    {
        try
        {
            logger.LogDebug("Create new key entry: " + key.Name, key.Id);
            var result = await repo.Create(key);
            if (!result)
            {
                logger.LogInformation("Could not save entity");
                return result;
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("Error was thrown by saving entity:" + ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateKeyAsync(KeyEntry newKey)
    {
        try
        {
            logger.LogDebug("Update entity " + newKey.Name);
            var result = await repo.Update(newKey);
            if (!result)
            {
                logger.LogInformation("Could not update entity");
                return result;
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("Error was thrown by updating entity " + ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteKeyAsync(KeyEntry key)
    {
        try
        {
            logger.LogDebug("Deleting entity " + key.Name);
            var result = await repo.Delete(key);
            if (!result)
            {
                logger.LogInformation("Could not delete entity");
                return result;
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("Error was thrown by deleting entity " + ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteKeyByIdAsync(int id)
    {
        try
        {
            logger.LogDebug("Deleting entity " + id);
            var result = await repo.DeleteById(id);
            if (!result)
            {
                logger.LogInformation("Could not delete entity by id");
                return result;
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("Error was thrown by deleting entity by id " + ex.Message);
            return false;
        }
    }
}