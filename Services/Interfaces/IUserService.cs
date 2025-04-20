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

        // New security and management methods
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
        Task<bool> LockUserAsync(int userId, TimeSpan duration);
        Task<bool> UnlockUserAsync(int userId);
        Task<bool> EnableTwoFactorAsync(int userId);
        Task<bool> DisableTwoFactorAsync(int userId);
        Task<bool> UpdateLastLoginAsync(int userId, string ipAddress);
        Task<bool> IncrementFailedLoginAttemptsAsync(int userId);
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 10);
    }
}
