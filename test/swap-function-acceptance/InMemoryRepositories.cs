using ChurchRota.SwapFunction.Models;
using ChurchRota.SwapFunction.Repositories;

namespace ChurchRota.SwapFunction.Acceptance.Tests;

/// <summary>
/// In-memory implementation of IScheduleRepository for testing.
/// </summary>
public class InMemoryScheduleRepository : IScheduleRepository
{
    private readonly Dictionary<string, ScheduleTableEntity> _schedules = new();

    public Task<ScheduleTableEntity?> GetByIdAsync(string scheduleId)
    {
        _schedules.TryGetValue(scheduleId, out var schedule);
        return Task.FromResult(schedule);
    }

    public Task<IEnumerable<ScheduleTableEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var results = _schedules.Values
            .Where(s => s.Date >= startDate && s.Date <= endDate)
            .ToList();
        return Task.FromResult<IEnumerable<ScheduleTableEntity>>(results);
    }

    public Task<ScheduleTableEntity> CreateAsync(ScheduleTableEntity entity)
    {
        _schedules[entity.ScheduleId] = entity;
        return Task.FromResult(entity);
    }

    public Task<ScheduleTableEntity> UpdateAsync(ScheduleTableEntity entity)
    {
        _schedules[entity.ScheduleId] = entity;
        return Task.FromResult(entity);
    }

    public Task<int> GetPersonAssignmentCountAsync(string personId, DateTime startDate, DateTime endDate)
    {
        var count = _schedules.Values
            .Count(s => s.PersonId == personId && s.Date >= startDate && s.Date <= endDate);
        return Task.FromResult(count);
    }
}

/// <summary>
/// In-memory implementation of IPeopleRepository for testing.
/// </summary>
public class InMemoryPeopleRepository : IPeopleRepository
{
    private readonly Dictionary<string, PersonTableEntity> _people = new();

    public Task<PersonTableEntity?> GetByIdAsync(string personId)
    {
        _people.TryGetValue(personId, out var person);
        return Task.FromResult(person);
    }

    public Task<IEnumerable<PersonTableEntity>> GetByRoleAsync(string roleId)
    {
        var results = _people.Values
            .Where(p => p.Roles?.Split(',', StringSplitOptions.RemoveEmptyEntries).Contains(roleId) == true)
            .ToList();
        return Task.FromResult<IEnumerable<PersonTableEntity>>(results);
    }

    public Task<PersonTableEntity> CreateAsync(PersonTableEntity entity)
    {
        _people[entity.PersonId] = entity;
        return Task.FromResult(entity);
    }

    public Task<PersonTableEntity> UpdateAsync(PersonTableEntity entity)
    {
        _people[entity.PersonId] = entity;
        return Task.FromResult(entity);
    }
}

/// <summary>
/// In-memory implementation of IAvailabilityRepository for testing.
/// </summary>
public class InMemoryAvailabilityRepository : IAvailabilityRepository
{
    private readonly Dictionary<string, AvailabilityTableEntity> _availability = new();

    private static string GetKey(string personId, DateTime date) => $"{personId}_{date:yyyyMMdd}";

    public Task<AvailabilityTableEntity?> GetAsync(string personId, DateTime date)
    {
        var key = GetKey(personId, date);
        _availability.TryGetValue(key, out var availability);
        return Task.FromResult(availability);
    }

    public Task<IEnumerable<AvailabilityTableEntity>> GetByPersonAsync(string personId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var results = _availability.Values
            .Where(a => a.PersonId == personId)
            .Where(a => !startDate.HasValue || a.Date >= startDate.Value)
            .Where(a => !endDate.HasValue || a.Date <= endDate.Value)
            .ToList();
        return Task.FromResult<IEnumerable<AvailabilityTableEntity>>(results);
    }

    public Task<AvailabilityTableEntity> SetAsync(AvailabilityTableEntity entity)
    {
        var key = GetKey(entity.PersonId, entity.Date);
        _availability[key] = entity;
        return Task.FromResult(entity);
    }

    public async Task SetBulkAsync(string personId, IEnumerable<AvailabilityTableEntity> entities)
    {
        foreach (var entity in entities)
        {
            await SetAsync(entity);
        }
    }
}
