using Microsoft.AspNetCore.Mvc.Rendering;
using Salon360App.Models;

namespace Salon360App.Services.Interfaces
{
    public interface ICustomerTypeService
    {
        Task<IEnumerable<CustomerType>> GetAllAsync();
        Task<CustomerType> GetByIdAsync(int id);
        Task<CustomerType> GetByNameAsync(string name);
        Task<bool> CreateAsync(CustomerType type);
        Task<bool> UpdateAsync(CustomerType type);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<IEnumerable<SelectListItem>> GetDropdownItemsAsync(bool includeDefault);

    }
}
