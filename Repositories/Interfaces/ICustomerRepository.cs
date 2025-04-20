using Microsoft.EntityFrameworkCore;
using Salon360App.Models;

namespace Salon360App.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetByUserIdAsync(int userId);
        Task<Customer> GetWithTypeAsync(int customerId);
        Task<IEnumerable<Customer>> GetAllWithTypesAsync();
        Task<IEnumerable<Customer>> GetByCustomerTypeAsync(int typeId);
        Task<IEnumerable<Customer>> GetRecentCustomersAsync(DateTime fromDate);
    }

    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(DbContext context) : base(context) { }

        public async Task<Customer> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Customer> GetWithTypeAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.CustomerType)
                .Include(c => c.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<IEnumerable<Customer>> GetAllWithTypesAsync()
        {
            return await _dbSet
                .Include(c => c.CustomerType)
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetByCustomerTypeAsync(int typeId)
        {
            return await _dbSet
                .Include(c => c.CustomerType)
                .Include(c => c.User)
                .Where(c => c.CustomerTypeId == typeId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetRecentCustomersAsync(DateTime fromDate)
        {
            return await _dbSet
                .Include(c => c.CustomerType)
                .Include(c => c.User)
                .Where(c => c.User.RegisteredAt >= fromDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
