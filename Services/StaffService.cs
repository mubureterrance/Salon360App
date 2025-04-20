using Salon360App.Models;
using Salon360App.Services.Interfaces;
using Salon360App.UnitOfWork;

namespace Salon360App.Services
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _unitOfWork.Staffs.GetAllWithRolesAsync();
        }

        public async Task<Staff> GetByIdAsync(int id)
        {
            return await _unitOfWork.Staffs.GetByIdAsync(id);
        }

        public async Task<Staff> GetWithRoleAsync(int id)
        {
            return await _unitOfWork.Staffs.GetWithRoleAsync(id);
        }

        public async Task<IEnumerable<Staff>> GetByRoleAsync(int roleId)
        {
            return await _unitOfWork.Staffs.GetByRoleAsync(roleId);
        }

        public async Task<IEnumerable<Staff>> GetRecentHiresAsync(DateTime since)
        {
            return await _unitOfWork.Staffs.GetRecentHiresAsync(since);
        }

        public async Task<bool> CreateAsync(Staff staff)
        {
            await _unitOfWork.Staffs.AddAsync(staff);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Staff staff)
        {
            _unitOfWork.Staffs.Update(staff);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(id);
            if (staff == null) return false;

            _unitOfWork.Staffs.Remove(staff);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
