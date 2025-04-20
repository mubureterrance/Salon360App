using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Salon360App.Models;
using System.Linq.Expressions;

namespace Salon360App.Data
{
    public class SalonDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public SalonDbContext(DbContextOptions<SalonDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffRole> StaffRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ignore BaseEntry as an entity (it's just metadata)
            builder.Ignore<BaseEntry>();

            // ---------------------------
            // One-to-one relationships
            // ---------------------------
            builder.Entity<User>()
                .HasOne(u => u.CustomerProfile)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                .HasOne(u => u.StaffProfile)
                .WithOne(s => s.User)
                .HasForeignKey<Staff>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------------------
            // Audit User Relationships
            // ---------------------------
            foreach (var entityType in builder.Model.GetEntityTypes()
                .Where(t => typeof(BaseEntry).IsAssignableFrom(t.ClrType) && !t.IsAbstract()))
            {
                builder.Entity(entityType.ClrType).HasOne(typeof(User), "CreatedBy")
                    .WithMany()
                    .HasForeignKey("CreatedById")
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity(entityType.ClrType).HasOne(typeof(User), "UpdatedBy")
                    .WithMany()
                    .HasForeignKey("UpdatedById")
                    .OnDelete(DeleteBehavior.Restrict);

                ApplySoftDeleteFilter(builder, entityType.ClrType);
            }

            // ---------------------------
            // Indexes
            // ---------------------------

            // Customer indexes
            builder.Entity<Customer>().HasIndex(c => c.UserId).IsUnique();
            builder.Entity<Customer>().HasIndex(c => c.CustomerTypeId);
            builder.Entity<Customer>().HasIndex(c => c.CreatedById);
            builder.Entity<Customer>().HasIndex(c => c.UpdatedById);
            builder.Entity<Customer>().HasIndex(c => c.IsActive)
                .HasFilter("[IsActive] = 1");

            // Staff indexes
            builder.Entity<Staff>().HasIndex(s => s.UserId).IsUnique();
            builder.Entity<Staff>().HasIndex(s => s.StaffRoleId);
            builder.Entity<Staff>().HasIndex(s => s.CreatedById);
            builder.Entity<Staff>().HasIndex(s => s.UpdatedById);
            builder.Entity<Staff>().HasIndex(s => s.IsActive)
                .HasFilter("[IsActive] = 1");

            // StaffRole + CustomerType indexes
            builder.Entity<StaffRole>().HasIndex(r => r.IsActive)
                .HasFilter("[IsActive] = 1");

            builder.Entity<CustomerType>().HasIndex(t => t.IsActive)
                .HasFilter("[IsActive] = 1");

            // ---------------------------
            builder.Entity<StaffRole>()
                .Property(r => r.BaseRate)
                .HasPrecision(10, 2); // means: max 99999999.99
        }

        // Dynamically apply soft delete filter to all entities inheriting BaseEntry
        private static void ApplySoftDeleteFilter(ModelBuilder builder, Type entityType)
        {
            var method = typeof(SalonDbContext)
                .GetMethod(nameof(ApplySoftDeleteFilterInternal), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .MakeGenericMethod(entityType);
            method.Invoke(null, new object[] { builder });
        }

        private static void ApplySoftDeleteFilterInternal<TEntity>(ModelBuilder builder) where TEntity : BaseEntry
        {
            builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
