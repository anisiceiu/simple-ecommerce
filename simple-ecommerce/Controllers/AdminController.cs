using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        public AdminController(IAuthService authService)
        {
              _authService = authService;  
        }
        public IActionResult Index()
        {
            var isAuth = User.Identity.IsAuthenticated; // should now be true
            var roles = User.IsInRole("Admin"); // true if assigned
            return View();
        }

        public IActionResult Customers()
        {
            return View();
        }

        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _authService.GetAllCustomersAsync();

            return Ok(customers);
        }
    }
}
