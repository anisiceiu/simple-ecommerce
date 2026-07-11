using ECommerce.Application.DTOs;
using ECommerce.Domain;
using Microsoft.AspNetCore.Mvc;
using ecommerce.Models;
using ecommerce.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace ecommerce.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "CART";
        private readonly IHubContext<CartHub> _cartHubContext;

        public CartController(IHubContext<CartHub> cartHubContext)
        {
            _cartHubContext = cartHubContext;
        }

        [HttpGet]
        public IActionResult GetCartSummary()
        {
            return ViewComponent("CartSummary");
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
                var newItem = new CartItemViewModel
                {
                    ProductId = id,
                    ProductName = name,
                    Price = price,
                    Quantity = 1
                };
                cart.Add(newItem);

                // Notify all clients in this user's group
                var userId = User.Identity?.Name ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
                _cartHubContext.Clients.Group($"user-{userId}")
                    .SendAsync("ItemAdded", newItem);
            }
            else
            {
                item.Quantity++;

                // Notify all clients in this user's group
                var userId = User.Identity?.Name ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
                _cartHubContext.Clients.Group($"user-{userId}")
                    .SendAsync("ItemUpdated", new { item.ProductId, item.Quantity });
            }

            SaveCart(cart);
            return NoContent();
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
                cart.Remove(item);

            SaveCart(cart);

            // Notify all clients in this user's group
            var userId = User.Identity?.Name ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            _cartHubContext.Clients.Group($"user-{userId}")
                .SendAsync("ItemRemoved", id);

            return RedirectToAction("Index", cart);
        }

        [HttpPost]
        public IActionResult Update([FromBody] CartUpdateDto dto)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == dto.Id);

            if (item != null)
                item.Quantity = dto.Quantity;

            SaveCart(cart);

            // Notify all clients in this user's group
            var userId = User.Identity?.Name ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            _cartHubContext.Clients.Group($"user-{userId}")
                .SendAsync("ItemUpdated", new { productId = dto.Id, quantity = dto.Quantity });

            return Ok();
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartKey);

            // Notify all clients in this user's group
            var userId = User.Identity?.Name ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            _cartHubContext.Clients.Group($"user-{userId}")
                .SendAsync("CartCleared");

            return RedirectToAction("Index");
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
