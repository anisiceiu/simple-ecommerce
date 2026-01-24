using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using simple_ecommerce.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace simple_ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private const string CartKey = "CART";
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        
        public HomeController(ILogger<HomeController> logger, IProductService productService,
            IOrderService orderService, IOrderItemService orderItemService)
        {
            _logger = logger;
            _productService = productService;
            _orderService = orderService;
            _orderItemService = orderItemService;
        }

        public async Task<IActionResult> Index(string sort = "", int page = 1, int pageSize = 5)
        {
            var productsQuery = (await _productService.GetProductsAsync()).AsQueryable();

            //SORTING
            productsQuery = sort switch
            {
                "asc" => productsQuery.OrderBy(p => p.Name),
                "desc" => productsQuery.OrderByDescending(p => p.Name),
                "price" => productsQuery.OrderBy(p => p.Price),
                _ => productsQuery.OrderBy(p => p.Id) // default
            };

            int totalItems = productsQuery.Count();

            var products = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentSort = sort; // to keep dropdown selected
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(products);
        }



        public async Task<IActionResult> ProductDetails(int id)
        {
            var products = await _productService.GetProductsAsync();

            var pro = products.FirstOrDefault(c => c.Id == id);

            return View(pro);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PlaceOrder( OrderAddress shippingAddress)
        {
            var cart = GetCart();

            if (cart == null || !cart.Any())
                return BadRequest("Cart is empty");

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Order order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                Status = OrderStatuses.Pending,
                TotalAmount = cart.Sum(c => c.Total),
                ShippingAddress = shippingAddress
            };

            var result = await _orderService.CreateAsync(order);

            if (result == null)
                return BadRequest("Order creation failed");

            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    OrderId = result.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };

                await _orderItemService.CreateAsync(orderItem);
            }

            //clear cart
            HttpContext.Session.Remove(CartKey);

            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<CartItemViewModel> GetCart()
        {
            return HttpContext.Session.Get<List<CartItemViewModel>>(CartKey)
                   ?? new List<CartItemViewModel>();
        }
    }
}
