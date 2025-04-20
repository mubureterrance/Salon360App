using Microsoft.AspNetCore.Mvc.Rendering;
using Salon360App.Models;

namespace Salon360App.Services.Interfaces
{
    public interface IStaffRoleService
    {
        Task<IEnumerable<StaffRole>> GetAllAsync();
        Task<StaffRole> GetByIdAsync(int id);
        Task<StaffRole> GetByCodeAsync(string code);
        Task<bool> CreateAsync(StaffRole role);
        Task<bool> UpdateAsync(StaffRole role);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<IEnumerable<SelectListItem>> GetDropdownItemsAsync(bool includeDefault);

    }
}
