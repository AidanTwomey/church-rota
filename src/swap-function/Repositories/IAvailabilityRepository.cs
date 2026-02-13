using ChurchRota.SwapFunction.Models;

namespace ChurchRota.SwapFunction.Repositories;

public interface IAvailabilityRepository
{
    Task<AvailabilityTableEntity?> GetAsync(string personId, DateTime date);
    Task<IEnumerable<AvailabilityTableEntity>> GetByPersonAsync(string personId, DateTime? startDate = null, DateTime? endDate = null);
    Task<AvailabilityTableEntity> SetAsync(AvailabilityTableEntity entity);
    Task SetBulkAsync(string personId, IEnumerable<AvailabilityTableEntity> entities);
}
