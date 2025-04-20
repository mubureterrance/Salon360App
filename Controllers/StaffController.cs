using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Salon360App.Data;
using Salon360App.Enums;
using Salon360App.Models;
using Salon360App.Services.Interfaces;
using Salon360App.ViewModels.StaffViewModels;

namespace Salon360App.Controllers
{
    public class StaffController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SalonDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IStaffRoleService _staffRoleService;
        private readonly IStaffService _staffService;

        public StaffController(
            ILogger<AccountController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            SalonDbContext context,
            IWebHostEnvironment env,
            IStaffRoleService staffRoleService,
            IStaffService staffService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _env = env;
            _staffRoleService = staffRoleService;
            _staffService = staffService;
        }

        public async Task<IActionResult> Index()
        {
            var staffEntities = await _staffService.GetAllAsync();

            var viewModelList = staffEntities.Select(s => new StaffIndexViewModel
            {
                StaffId = s.StaffId,
                FullName = s.User?.Firstname + " " + s.User?.Lastname,
                Email = s.User?.Email,
                RoleName = s.StaffRole?.Name,
                HireDate = s.HireDate,
                Bio = s.Bio
            }).ToList();

            return View(viewModelList);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var staff = await _staffService.GetWithRoleAsync(id);

                if (staff == null)
                {
                    _logger.LogWarning("Staff with ID {StaffId} not found", id);
                    return NotFound();
                }

                if (staff.User == null || staff.StaffRole == null)
                {
                    _logger.LogWarning("Incomplete staff data for ID {StaffId} (User: {HasUser}, Role: {HasRole})",
                        id, staff.User != null, staff.StaffRole != null);
                    return NotFound();
                }

                var model = new StaffDetailsViewModel
                {
                    StaffId = staff.StaffId,
                    FullName = $"{staff.User.Firstname} {staff.User.Lastname}",
                    Email = staff.User.Email,
                    RoleName = staff.StaffRole.Name,
                    Bio = staff.Bio,
                    HireDate = staff.HireDate,
                    Gender = staff.User.Gender.GetDisplayName(),
                    DateOfBirth = staff.User.DateOfBirth,
                    Address = staff.User.Address,
                    ProfileImageUrl = staff.User.ProfileImageUrl ?? "/images/default-profile.png",
                    IsActive = staff.IsActive
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff details for ID {StaffId}", id);
                return StatusCode(500, "An error occurred while retrieving staff details");
            }
        }
    }
}
