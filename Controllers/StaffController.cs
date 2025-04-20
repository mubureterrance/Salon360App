using Microsoft.AspNetCore.Mvc;
using Salon360App.Services.Interfaces;

namespace Salon360App.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        public async Task<IActionResult> Index()
        {
            var staff = await _staffService.GetAllAsync();
            return View(staff);
        }

        public async Task<IActionResult> Details(int id)
        {
            var staffMember = await _staffService.GetByIdAsync(id);
            if (staffMember == null) return NotFound();
            return View(staffMember);
        }
    }
}
