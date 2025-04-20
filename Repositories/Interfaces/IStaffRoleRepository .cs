using Microsoft.EntityFrameworkCore;
using Salon360App.Models;

namespace Salon360App.Repositories.Interfaces
{
    public interface IStaffRoleRepository : IRepository<StaffRole>
    {
        Task<StaffRole> GetByNameAsync(string name);
        Task<StaffRole> GetByCodeAsync(string code);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<IEnumerable<StaffRole>> GetAllWithStaffCountsAsync();
    }

    public class StaffRoleRepository : Repository<StaffRole>, IStaffRoleRepository
    {
        public StaffRoleRepository(DbContext context) : base(context) { }

        public async Task<StaffRole> GetByNameAsync(string name)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<StaffRole> GetByCodeAsync(string code)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == code);
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            return !await _dbSet
                .AnyAsync(r => r.Name == name && (!excludeId.HasValue || r.StaffRoleId != excludeId.Value));
        }

        public async Task<IEnumerable<StaffRole>> GetAllWithStaffCountsAsync()
        {
            return await _dbSet
                .Include(r => r.Staffs)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
