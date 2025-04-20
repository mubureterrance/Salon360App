using Microsoft.AspNetCore.Mvc;
using Salon360App.Services.Interfaces;
using Salon360App.ViewModels.StaffViewModels;

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
            var staffMember = await _staffService.GetByIdAsync(id);
            if (staffMember == null) return NotFound();
            return View(staffMember);
        }
    }
}
