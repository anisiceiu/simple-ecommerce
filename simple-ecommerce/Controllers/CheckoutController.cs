using Microsoft.AspNetCore.Mvc;
using simple_ecommerce.Models;
using System.Globalization;

namespace simple_ecommerce.Controllers
{
    public class CheckoutController : Controller
    {
        private const string CartKey = "CART";
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }


        private List<CartItemViewModel> GetCart()
        {
            return HttpContext.Session.Get<List<CartItemViewModel>>(CartKey)
                   ?? new List<CartItemViewModel>();
        }
    }
}
