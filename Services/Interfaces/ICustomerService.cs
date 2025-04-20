using Salon360App.Models;

namespace Salon360App.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> GetWithTypeAsync(int id);
        Task<IEnumerable<Customer>> GetByTypeAsync(int typeId);
        Task<IEnumerable<Customer>> GetRecentCustomersAsync(DateTime since);
        Task<bool> CreateAsync(Customer customer);
        Task<bool> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
    }
}
