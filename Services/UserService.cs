using Microsoft.AspNetCore.Identity;
using Salon360App.Models;
using Salon360App.Services.Interfaces;
using Salon360App.UnitOfWork;

namespace Salon360App.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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
    }
}
