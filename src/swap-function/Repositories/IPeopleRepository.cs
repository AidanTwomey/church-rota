using ChurchRota.SwapFunction.Models;

namespace ChurchRota.SwapFunction.Repositories;

public interface IPeopleRepository
{
    Task<PersonTableEntity?> GetByIdAsync(string personId);
    Task<IEnumerable<PersonTableEntity>> GetByRoleAsync(string roleId);
    Task<PersonTableEntity> CreateAsync(PersonTableEntity entity);
    Task<PersonTableEntity> UpdateAsync(PersonTableEntity entity);
}
