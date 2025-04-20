using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Salon360App.Data;
using Salon360App.Models;
using Salon360App.UnitOfWork;
using Salon360App.Services.Interfaces;
using Salon360App.Services;

namespace Salon360App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure DbContext
            builder.Services.AddDbContext<SalonDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Identity
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<SalonDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IStaffService, StaffService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IStaffRoleService, StaffRoleService>();
            builder.Services.AddScoped<ICustomerTypeService, CustomerTypeService>();

            // Register seeder
            builder.Services.AddTransient<DataSeeder>();

            var app = builder.Build();

            // Seed default data
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await seeder.SeedAsync();
            }

            // Configure middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}
