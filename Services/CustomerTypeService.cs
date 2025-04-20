using Microsoft.AspNetCore.Mvc.Rendering;
using Salon360App.Models;
using Salon360App.Services.Interfaces;
using Salon360App.UnitOfWork;
using System.Data;

namespace Salon360App.Services
{
    public class CustomerTypeService : ICustomerTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CustomerType>> GetAllAsync()
        {
            return await _unitOfWork.CustomerTypes.GetAllWithCustomerCountsAsync();
        }

        public async Task<CustomerType> GetByIdAsync(int id)
        {
            return await _unitOfWork.CustomerTypes.GetByIdAsync(id);
        }

        public async Task<CustomerType> GetByNameAsync(string name)
        {
            return await _unitOfWork.CustomerTypes.GetByNameAsync(name);
        }

        public async Task<bool> CreateAsync(CustomerType type)
        {
            await _unitOfWork.CustomerTypes.AddAsync(type);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(CustomerType type)
        {
            _unitOfWork.CustomerTypes.Update(type);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var type = await _unitOfWork.CustomerTypes.GetByIdAsync(id);
            if (type == null || (type.Customers?.Any() ?? false))
                return false;

            _unitOfWork.CustomerTypes.Remove(type);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            return await _unitOfWork.CustomerTypes.IsNameUniqueAsync(name, excludeId);
        }

        public async Task<IEnumerable<SelectListItem>> GetDropdownItemsAsync(bool includeDefault = true)
        {
            var types = await _unitOfWork.CustomerTypes.GetAllAsync();

            var items = types
                .OrderBy(t => t.Name)
                .Select(t => new SelectListItem
                {
                    Value = t.CustomerTypeId.ToString(),
                    Text = t.Name
                }).ToList();

            if (includeDefault)
                items.Insert(0, new SelectListItem { Value = "", Text = "-- Select Customer Type --" });

            return items;
        }

    }
}
