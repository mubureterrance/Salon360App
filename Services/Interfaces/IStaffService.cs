using Salon360App.Models;

namespace Salon360App.Services.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<Staff> GetByIdAsync(int id);
        Task<Staff> GetWithRoleAsync(int id);
        Task<IEnumerable<Staff>> GetByRoleAsync(int roleId);
        Task<IEnumerable<Staff>> GetRecentHiresAsync(DateTime since);
        Task<bool> CreateAsync(Staff staff);
        Task<bool> UpdateAsync(Staff staff);
        Task<bool> DeleteAsync(int id);

    }

}
