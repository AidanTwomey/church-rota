using ChurchRota.SwapFunction.Models;

namespace ChurchRota.SwapFunction.Repositories;

public interface IScheduleRepository
{
    Task<ScheduleTableEntity?> GetByIdAsync(string scheduleId);
    Task<IEnumerable<ScheduleTableEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<ScheduleTableEntity> CreateAsync(ScheduleTableEntity entity);
    Task<ScheduleTableEntity> UpdateAsync(ScheduleTableEntity entity);
    Task<int> GetPersonAssignmentCountAsync(string personId, DateTime startDate, DateTime endDate);
}
