using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Salon360App.Enums;
using Salon360App.Models;
using System.Data;
using System.Security.Claims;

namespace Salon360App.Data
{
    public class DataSeeder
    {
        private readonly SalonDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public DataSeeder(
            SalonDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await SeedRolesAsync();
            await SeedCustomerTypesAsync();
            await SeedStaffRolesAsync();
            await SeedAdminUserAsync();
        }

        private async Task SeedRolesAsync()
        {
            foreach (var roleName in Enum.GetNames(typeof(UserType)))
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
        }

        private async Task SeedCustomerTypesAsync()
        {
            if (!_context.CustomerTypes.Any())
            {
                var types = Enum.GetValues(typeof(DefaultCustomerType))
                    .Cast<DefaultCustomerType>()
                    .Select(type => new CustomerType
                    {
                        Code = type.ToString(),
                        Name = type.GetType().GetMember(type.ToString())[0]
                            .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                            .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                            .FirstOrDefault()?.Name ?? type.ToString(),
                        Description = $"{type} customer type"
                    })
                    .ToList(); // Evaluate immediately so we can log below

                foreach (var type in types)
                {
                    Console.WriteLine($"✔ Seeded CustomerType: {type.Name}");
                }

                await _context.CustomerTypes.AddRangeAsync(types);
                await _context.SaveChangesAsync();
            }
        }


        private async Task SeedStaffRolesAsync()
        {
            if (!_context.StaffRoles.Any())
            {
                var roles = Enum.GetValues(typeof(DefaultStaffRole))
                    .Cast<DefaultStaffRole>()
                    .Select(role => new StaffRole
                    {
                        Code = role.ToString(),
                        Name = role.GetType().GetMember(role.ToString())[0]
                            .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                            .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                            .FirstOrDefault()?.Name ?? role.ToString(),
                        Description = $"{role} seeded from enum"
                    })
                    .ToList(); // Evaluate immediately so we can log below

                foreach (var role in roles)
                {
                    Console.WriteLine($"✔ Seeded StaffRole: {role.Name}");
                }

                    await _context.StaffRoles.AddRangeAsync(roles);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedAdminUserAsync()
        {
            const string adminEmail = "admin@salon360.com";
            const string adminPassword = "Admin@123";

            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null) return;

            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                Firstname = "System",
                Lastname = "Administrator",
                UserType = UserType.Admin,
                Gender = Gender.PreferNotToSay,
                RegisteredAt = DateTime.UtcNow,
                Address = "System HQ" 
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Assign role
                await _userManager.AddToRoleAsync(adminUser, UserType.Admin.ToString());

                // 🔥 Add custom claims
                await _userManager.AddClaimAsync(adminUser, new Claim("given_name", adminUser.Firstname));
                await _userManager.AddClaimAsync(adminUser, new Claim("family_name", adminUser.Lastname));
                await _userManager.AddClaimAsync(adminUser, new Claim("role", UserType.Admin.ToString()));
            }
        }
    }
}
