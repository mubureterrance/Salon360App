using Microsoft.AspNetCore.Mvc.Rendering;
using Salon360App.Models;
using Salon360App.Services.Interfaces;
using Salon360App.UnitOfWork;

namespace Salon360App.Services
{
    public class StaffRoleService : IStaffRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffRoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StaffRole>> GetAllAsync()
        {
            return await _unitOfWork.StaffRoles.GetAllWithStaffCountsAsync();
        }

        public async Task<StaffRole> GetByIdAsync(int id)
        {
            return await _unitOfWork.StaffRoles.GetByIdAsync(id);
        }

        public async Task<StaffRole> GetByCodeAsync(string code)
        {
            return await _unitOfWork.StaffRoles.GetByCodeAsync(code);
        }

        public async Task<bool> CreateAsync(StaffRole role)
        {
            await _unitOfWork.StaffRoles.AddAsync(role);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(StaffRole role)
        {
            _unitOfWork.StaffRoles.Update(role);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _unitOfWork.StaffRoles.GetByIdAsync(id);
            if (role == null || (role.Staffs?.Any() ?? false))
                return false;

            _unitOfWork.StaffRoles.Remove(role);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            return await _unitOfWork.StaffRoles.IsNameUniqueAsync(name, excludeId);
        }


        public async Task<IEnumerable<SelectListItem>> GetDropdownItemsAsync(bool includeDefault = true)
        {
            var roles = await _unitOfWork.StaffRoles.GetAllAsync();

            var items = roles
                .OrderBy(r => r.Name)
                .Select(r => new SelectListItem
                {
                    Value = r.StaffRoleId.ToString(),
                    Text = r.Name
                }).ToList();

            if (includeDefault)
                items.Insert(0, new SelectListItem { Value = "", Text = "-- Select Staff Role --" });

            return items;
        }

    }
}
