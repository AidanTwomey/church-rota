using Azure.Data.Tables;
using ChurchRota.SwapFunction.Models;

namespace ChurchRota.SwapFunction.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly TableClient _tableClient;

    public ScheduleRepository()
    {
        var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");
        _tableClient = new TableClient(connectionString, "Schedules");
        _tableClient.CreateIfNotExists();
    }

    public async Task<ScheduleTableEntity?> GetByIdAsync(string scheduleId)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<ScheduleTableEntity>("Schedule", scheduleId);
            return response.Value;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<IEnumerable<ScheduleTableEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var startDateStr = startDate.ToString("yyyy-MM-dd");
        var endDateStr = endDate.ToString("yyyy-MM-dd");
        
        var filter = $"PartitionKey eq 'Schedule' and Date ge datetime'{startDateStr}' and Date le datetime'{endDateStr}'";
        
        var results = new List<ScheduleTableEntity>();
        await foreach (var entity in _tableClient.QueryAsync<ScheduleTableEntity>(filter))
        {
            results.Add(entity);
        }
        
        return results;
    }

    public async Task<ScheduleTableEntity> CreateAsync(ScheduleTableEntity entity)
    {
        entity.PartitionKey = "Schedule";
        entity.RowKey = entity.ScheduleId;
        await _tableClient.AddEntityAsync(entity);
        return entity;
    }

    public async Task<ScheduleTableEntity> UpdateAsync(ScheduleTableEntity entity)
    {
        await _tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
        return entity;
    }

    public async Task<int> GetPersonAssignmentCountAsync(string personId, DateTime startDate, DateTime endDate)
    {
        var startDateStr = startDate.ToString("yyyy-MM-dd");
        var endDateStr = endDate.ToString("yyyy-MM-dd");
        
        var filter = $"PartitionKey eq 'Schedule' and PersonId eq '{personId}' and Date ge datetime'{startDateStr}' and Date le datetime'{endDateStr}'";
        
        var count = 0;
        await foreach (var _ in _tableClient.QueryAsync<ScheduleTableEntity>(filter))
        {
            count++;
        }
        
        return count;
    }
}
