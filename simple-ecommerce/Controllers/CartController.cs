using Microsoft.AspNetCore.Mvc;
using simple_ecommerce.Models;
using System.Text.Json;

namespace simple_ecommerce.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "CART";

        public IActionResult GetCartSummary()
        {
            var cartJson = HttpContext.Session.GetString("CART");
            var cart = string.IsNullOrEmpty(cartJson)
                ? new List<CartItemViewModel>()
                : JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson);

            var totalItems = cart.Sum(x => x.Quantity);
            var totalAmount = cart.Sum(x => x.Price * x.Quantity);

           return Json(new
            {
                count = totalItems,
                total = totalAmount
            });
        }
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        public IActionResult Add(int id, string name, decimal price)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item == null)
            {
                cart.Add(new CartItemViewModel
                {
                    ProductId = id,
                    ProductName = name,
                    Price = price,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }

            SaveCart(cart);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
                cart.Remove(item);

            SaveCart(cart);
            return NoContent();
        }

        public IActionResult Update(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
                item.Quantity = quantity;

            SaveCart(cart);
            return NoContent();
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartKey);
            return NoContent();
        }

        private List<CartItemViewModel> GetCart()
        {
            return HttpContext.Session.Get<List<CartItemViewModel>>(CartKey)
                   ?? new List<CartItemViewModel>();
        }

        private void SaveCart(List<CartItemViewModel> cart)
        {
            HttpContext.Session.Set(CartKey, cart);
        }
    }

}
