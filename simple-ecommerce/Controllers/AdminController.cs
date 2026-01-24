using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;
        public AdminController(IAuthService authService, IOrderService orderService)
        {
              _authService = authService;
            _orderService = orderService;
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

        public IActionResult Orders()
        {
            return View();
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var orderDetails = await _orderService.GetByIdAsync(id);
            return View(orderDetails);
        }

        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _authService.GetAllCustomersAsync();

            return Ok(customers);
        }

        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrderForListAsync();

            return Ok(orders);
        }
    }
}
