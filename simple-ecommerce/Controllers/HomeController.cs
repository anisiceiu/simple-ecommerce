using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ecommerce.Hubs;
using ecommerce.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private const string CartKey = "CART";
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly ICategoryService _categoryService;
        private readonly IHubContext<AdminNotificationHub> _hub;
        private readonly IAuthService _authService;

        public HomeController(ILogger<HomeController> logger, IProductService productService,IAuthService authService,
            IOrderService orderService, IOrderItemService orderItemService, ICategoryService categoryService, IHubContext<AdminNotificationHub> hub)
        {
            _logger = logger;
            _productService = productService;
            _orderService = orderService;
            _orderItemService = orderItemService;
            _categoryService = categoryService;
            _hub = hub;
            _authService = authService;
        }

        public async Task<IActionResult> Index(
                    List<int> categories = null,
                    string search = "",
                    string sort = "",
                    int page = 1,
                    int pageSize = 5)
        {
            // 🔹 Categories for sidebar
            ViewBag.Categories = (await _categoryService.GetAllAsync())
                                    .Where(c => c.IsActive)
                                    .ToList();

            ViewBag.SelectedCategories = categories;

            // 🔹 Start query (better if service returns IQueryable)
            var productsQuery = (await _productService.GetProductsAsync()).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(search.ToLower()));


            // ✅ CATEGORY FILTER (MISSING PART)
            if (categories != null && categories.Any())
                productsQuery = productsQuery.Where(p => categories.Contains(p.CategoryId));

            // ✅ SORT
            productsQuery = sort switch
            {
                "asc" => productsQuery.OrderBy(p => p.Name),
                "desc" => productsQuery.OrderByDescending(p => p.Name),
                "price" => productsQuery.OrderBy(p => p.Price),
                _ => productsQuery.OrderBy(p => p.Id)
            };

            int totalItems = productsQuery.Count();

            var products = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 🔹 Load ratings for displayed products
            var productIds = products.Select(p => p.Id).ToList();
            if (productIds.Any())
            {
                var ratingsData = await _productService.GetProductRatingsForListAsync(productIds);
                foreach (var product in products)
                {
                    if (ratingsData.TryGetValue(product.Id, out var ratingInfo))
                    {
                        product.AverageRating = ratingInfo.AverageRating;
                        product.TotalRatings = ratingInfo.TotalRatings;
                    }
                }
            }

            ViewBag.CurrentSort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var orderDetails = await _orderService.GetByIdAsync(id);
            return View(orderDetails);
        }


        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var ratings = await _productService.GetProductRatingsAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRating = await _productService.GetUserRatingAsync(id, userId);
            var averageRating = await _productService.GetAverageRatingAsync(id);
            var ratingDistribution = await _productService.GetRatingDistributionAsync(id);
            var users = await _authService.GetAllUsersAsync();
            Dictionary<string, string> dictionaryUser = users.ToDictionary(
                    p => p.Id,
                    p => p.PhoneNumber
                );

            // Map ratings to ReviewVM (need to get user names from identity context)
            var reviews = ratings.Select(r => new ProductRatingVM
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                UserDisplayName = dictionaryUser[r.UserId] // Placeholder - would need user lookup
            }).ToList();

            var vm = new ProductDetailsVM
            {
                Product = new ProductDto { 
                    CategoryId = product.CategoryId,
                    CreatedAt = product.CreatedAt,
                    Description = product.Description,
                    Id = product.Id,
                    ImageUrl = product.ImageUrl,
                    IsActive = product.IsActive,
                    Price = product.Price,
                    Name = product.Name,
                    Stock = product.Stock
                },
                AverageRating = averageRating,
                UserRating = (int?)userRating,
                TotalRatings = ratings.Count(),
                Reviews = reviews,
                RatingDistribution = ratingDistribution
            };
            return View(vm);
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
            await _hub.Clients.Group("Admins")
                        .SendAsync("NewOrderPlaced");


            return RedirectToAction("OrderSuccess");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _orderService.GetByIdAsync(orderId);

            if (order == null)
                return NotFound();

            if (order.Status == OrderStatuses.Pending)
            {
                await _orderService.UpdateStatusAsync(orderId, status);

                return RedirectToAction("OrderDetails", new { id = orderId });
            }
            else
            {
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult FAQ()
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

        [Authorize]
        public async Task<IActionResult> MyOrders(int page = 1)
        {
            int pageSize = 5;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ordersQuery = _orderService.GetOrdersIQueryable(userId)
                .AsNoTracking();

            var orders = await PaginatedList<Order>.CreateAsync(ordersQuery, page, pageSize);

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Rate(int productId, int rating)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _productService.RateProduct(userId, productId, rating);

            return Ok();
        }
    }
}
