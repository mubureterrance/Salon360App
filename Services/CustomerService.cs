using Salon360App.Models;
using Salon360App.Services.Interfaces;
using Salon360App.UnitOfWork;

namespace Salon360App.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _unitOfWork.Customers.GetAllWithTypesAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _unitOfWork.Customers.GetByIdAsync(id);
        }

        public async Task<Customer> GetWithTypeAsync(int id)
        {
            return await _unitOfWork.Customers.GetWithTypeAsync(id);
        }

        public async Task<IEnumerable<Customer>> GetByTypeAsync(int typeId)
        {
            return await _unitOfWork.Customers.GetByCustomerTypeAsync(typeId);
        }

        public async Task<IEnumerable<Customer>> GetRecentCustomersAsync(DateTime since)
        {
            return await _unitOfWork.Customers.GetRecentCustomersAsync(since);
        }

        public async Task<bool> CreateAsync(Customer customer)
        {
            await _unitOfWork.Customers.AddAsync(customer);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            _unitOfWork.Customers.Update(customer);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return false;

            _unitOfWork.Customers.Remove(customer);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
