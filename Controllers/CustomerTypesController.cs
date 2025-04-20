using Microsoft.AspNetCore.Mvc;
using Salon360App.Services.Interfaces;

namespace Salon360App.Controllers
{
    public class CustomerTypesController : Controller
    {
        private readonly ICustomerTypeService _customerTypeService;

        public CustomerTypesController(ICustomerTypeService customerTypeService)
        {
            _customerTypeService = customerTypeService;
        }

        public async Task<IActionResult> Index()
        {
            var types = await _customerTypeService.GetAllAsync();
            return View(types);
        }
    }
}
