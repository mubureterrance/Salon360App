using Microsoft.EntityFrameworkCore;
using Salon360App.Models;
using System.Linq.Expressions;

namespace Salon360App.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetWithProfileAsync(int id);
        Task<User> GetByUsernameAsync(string username); // Optional bonus
        Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate);
    }

    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> GetWithProfileAsync(int id)
        {
            return await _dbSet
                .Include(u => u.CustomerProfile)
                .Include(u => u.StaffProfile)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
