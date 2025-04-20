using Microsoft.AspNetCore.Identity;
using Salon360App.Models;
using Salon360App.Services.Interfaces;
using Salon360App.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Salon360App.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _unitOfWork.Users.GetAllAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email);
        }

        public async Task<User> GetWithProfileAsync(int id)
        {
            return await _unitOfWork.Users.GetWithProfileAsync(id);
        }

        public async Task<bool> CreateAsync(User user, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return false;

            await _userManager.AddToRoleAsync(user, role);
            return true;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _unitOfWork.Users.Update(user);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return false;

            _unitOfWork.Users.Remove(user);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                user.LastPasswordChangeDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            return result.Succeeded;
        }

        public async Task<bool> LockUserAsync(int userId, TimeSpan duration)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.LockoutEndDate = DateTime.UtcNow.Add(duration);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UnlockUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.LockoutEndDate = null;
            user.FailedLoginAttempts = 0;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> EnableTwoFactorAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.TwoFactorEnabled = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DisableTwoFactorAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.TwoFactorEnabled = false;
            user.TwoFactorSecret = null;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId, string ipAddress)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.LastLoginDate = DateTime.UtcNow;
            user.LastLoginIP = ipAddress;
            user.FailedLoginAttempts = 0;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> IncrementFailedLoginAttemptsAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.FailedLoginAttempts++;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            var users = await _unitOfWork.Users.FindAsync(u =>
                u.Firstname.Contains(searchTerm) ||
                u.Lastname.Contains(searchTerm) ||
                u.Email.Contains(searchTerm) ||
                u.UserName.Contains(searchTerm));

            return users
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
