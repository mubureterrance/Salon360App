using Microsoft.EntityFrameworkCore;
using Salon360App.Models;

namespace Salon360App.Repositories.Interfaces
{
    public interface ICustomerTypeRepository : IRepository<CustomerType>
    {
        Task<CustomerType> GetByNameAsync(string name);
        Task<CustomerType> GetByCodeAsync(string code); // if you use a 'Code' property
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<IEnumerable<CustomerType>> GetAllWithCustomerCountsAsync();
    }

    public class CustomerTypeRepository : Repository<CustomerType>, ICustomerTypeRepository
    {
        public CustomerTypeRepository(DbContext context) : base(context) { }

        public async Task<CustomerType> GetByNameAsync(string name)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(ct => ct.Name == name);
        }

        public async Task<CustomerType> GetByCodeAsync(string code)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(ct => ct.Code == code);
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            return !await _dbSet
                .AnyAsync(ct => ct.Name == name && (!excludeId.HasValue || ct.CustomerTypeId != excludeId.Value));
        }

        public async Task<IEnumerable<CustomerType>> GetAllWithCustomerCountsAsync()
        {
            return await _dbSet
                .Include(ct => ct.Customers)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
