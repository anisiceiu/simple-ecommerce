using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace simple_ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var isAuth = User.Identity.IsAuthenticated; // should now be true
            var roles = User.IsInRole("Admin"); // true if assigned
            return View();
        }
    }
}
