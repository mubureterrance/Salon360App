using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Salon360App.Models;

namespace Salon360App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // Example redirect logic based on roles or UserType
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "AdminDashboard");
            if (User.IsInRole("Staff"))
                return RedirectToAction("Index", "StaffDashboard");

            return RedirectToAction("Index", "CustomerDashboard");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
