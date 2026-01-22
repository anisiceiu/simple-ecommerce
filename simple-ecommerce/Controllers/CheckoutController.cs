using Microsoft.AspNetCore.Mvc;

namespace simple_ecommerce.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
