using Azure.Data.Tables;
using ChurchRota.SwapFunction.Models;

namespace ChurchRota.SwapFunction.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly TableClient _tableClient;

    public AvailabilityRepository()
    {
        var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");
        _tableClient = new TableClient(connectionString, "Availability");
        _tableClient.CreateIfNotExists();
    }

    public async Task<AvailabilityTableEntity?> GetAsync(string personId, DateTime date)
    {
        var rowKey = date.ToString("yyyyMMdd");
        try
        {
            var response = await _tableClient.GetEntityAsync<AvailabilityTableEntity>(personId, rowKey);
            return response.Value;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<IEnumerable<AvailabilityTableEntity>> GetByPersonAsync(string personId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var filter = $"PartitionKey eq '{personId}'";
        
        if (startDate.HasValue)
        {
            var startRowKey = startDate.Value.ToString("yyyyMMdd");
            filter += $" and RowKey ge '{startRowKey}'";
        }
        
        if (endDate.HasValue)
        {
            var endRowKey = endDate.Value.ToString("yyyyMMdd");
            filter += $" and RowKey le '{endRowKey}'";
        }
        
        var results = new List<AvailabilityTableEntity>();
        await foreach (var entity in _tableClient.QueryAsync<AvailabilityTableEntity>(filter))
        {
            results.Add(entity);
        }
        
        return results;
    }

    public async Task<AvailabilityTableEntity> SetAsync(AvailabilityTableEntity entity)
    {
        entity.PartitionKey = entity.PersonId;
        entity.RowKey = entity.Date.ToString("yyyyMMdd");
        await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        return entity;
    }

    public async Task SetBulkAsync(string personId, IEnumerable<AvailabilityTableEntity> entities)
    {
        var tasks = entities.Select(entity =>
        {
            entity.PartitionKey = personId;
            entity.RowKey = entity.Date.ToString("yyyyMMdd");
            return _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        });
        
        await Task.WhenAll(tasks);
    }
}
