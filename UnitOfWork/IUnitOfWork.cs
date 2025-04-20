using Salon360App.Data;
using Salon360App.Repositories;
using Salon360App.Repositories.Interfaces;

namespace Salon360App.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IStaffRepository Staffs { get; }
        ICustomerRepository Customers { get; }
        ICustomerTypeRepository CustomerTypes { get; }
        IStaffRoleRepository StaffRoles { get; }
        //IAccountRepository AccountRepository { get; }

        Task<int> CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly SalonDbContext _context;
        public IUserRepository Users { get; }
        public IStaffRepository Staffs { get; }
        public ICustomerRepository Customers { get; }
        public ICustomerTypeRepository CustomerTypes { get; }
        public IStaffRoleRepository StaffRoles { get; }
        //public IAccountRepository AccountRepository { get; }

        public UnitOfWork(SalonDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Staffs = new StaffRepository(_context);
            Customers = new CustomerRepository(_context);
            CustomerTypes = new CustomerTypeRepository(_context);
            StaffRoles = new StaffRoleRepository(_context);
            //AccountRepository = new AccountRepository(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
