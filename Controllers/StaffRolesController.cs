using Microsoft.AspNetCore.Mvc;
using Salon360App.Services.Interfaces;

namespace Salon360App.Controllers
{
    public class StaffRolesController : Controller
    {
        private readonly IStaffRoleService _staffRoleService;

        public StaffRolesController(IStaffRoleService staffRoleService)
        {
            _staffRoleService = staffRoleService;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _staffRoleService.GetAllAsync();
            return View(roles);
        }
    }
}
