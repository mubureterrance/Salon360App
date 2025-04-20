using Salon360App.Models;

namespace Salon360App.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetWithProfileAsync(int id);
        Task<bool> CreateAsync(User user, string password, string role);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
    }
}
