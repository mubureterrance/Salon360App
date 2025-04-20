using Microsoft.EntityFrameworkCore;
using Salon360App.Models;

namespace Salon360App.Repositories.Interfaces
{
    public interface IStaffRepository : IRepository<Staff>
    {
        Task<Staff> GetByUserIdAsync(int userId);
        Task<Staff> GetWithRoleAsync(int staffId);
        Task<IEnumerable<Staff>> GetAllWithRolesAsync();
        Task<IEnumerable<Staff>> GetByRoleAsync(int roleId);
        Task<IEnumerable<Staff>> GetRecentHiresAsync(DateTime fromDate);
    }

    public class StaffRepository : Repository<Staff>, IStaffRepository
    {
        public StaffRepository(DbContext context) : base(context) { }

        public async Task<Staff> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<Staff> GetWithRoleAsync(int staffId)
        {
            return await _dbSet
                .Include(s => s.StaffRole)
                .Include(s => s.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == staffId);
        }

        public async Task<IEnumerable<Staff>> GetAllWithRolesAsync()
        {
            return await _dbSet
                .Include(s => s.StaffRole)
                .Include(s => s.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> GetByRoleAsync(int roleId)
        {
            return await _dbSet
                .Include(s => s.StaffRole)
                .Include(s => s.User)
                .Where(s => s.StaffRoleId == roleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> GetRecentHiresAsync(DateTime fromDate)
        {
            return await _dbSet
                .Include(s => s.StaffRole)
                .Include(s => s.User)
                .Where(s => s.HireDate >= fromDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
