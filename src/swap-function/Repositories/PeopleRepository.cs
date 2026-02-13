using Azure.Data.Tables;
using ChurchRota.SwapFunction.Models;

namespace ChurchRota.SwapFunction.Repositories;

public class PeopleRepository : IPeopleRepository
{
    private readonly TableClient _tableClient;

    public PeopleRepository()
    {
        var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");
        _tableClient = new TableClient(connectionString, "People");
        _tableClient.CreateIfNotExists();
    }

    public async Task<PersonTableEntity?> GetByIdAsync(string personId)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<PersonTableEntity>("Person", personId);
            return response.Value;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<IEnumerable<PersonTableEntity>> GetByRoleAsync(string roleId)
    {
        var results = new List<PersonTableEntity>();
        
        await foreach (var entity in _tableClient.QueryAsync<PersonTableEntity>(e => e.PartitionKey == "Person"))
        {
            // Check if the person has the specified role (roles are comma-separated)
            var roles = entity.Roles?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            if (roles.Contains(roleId))
            {
                results.Add(entity);
            }
        }
        
        return results;
    }

    public async Task<PersonTableEntity> CreateAsync(PersonTableEntity entity)
    {
        entity.PartitionKey = "Person";
        entity.RowKey = entity.PersonId;
        await _tableClient.AddEntityAsync(entity);
        return entity;
    }

    public async Task<PersonTableEntity> UpdateAsync(PersonTableEntity entity)
    {
        await _tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
        return entity;
    }
}
