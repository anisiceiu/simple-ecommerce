using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;
        private readonly IInventoryTransactionService _inventoryService;
        private readonly IProductService _productService;
        public AdminController(IAuthService authService, IOrderService orderService, IInventoryTransactionService inventoryService, IProductService productService)
        {
              _authService = authService;
            _orderService = orderService;
            _inventoryService = inventoryService;
            _productService = productService;
        }
        public IActionResult Index()
        {
            var isAuth = User.Identity.IsAuthenticated; // should now be true
            var roles = User.IsInRole("Admin"); // true if assigned
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _orderService.GetByIdAsync(orderId);

            if (order == null)
                return NotFound();

            await _orderService.UpdateStatusAsync(orderId,status);
           
           if(order.Status != "Delivered" && status == "Delivered")
            {

                foreach (var item in order.OrderItems)
                {
                    // Process each order item
                    var product = await _productService.GetByIdAsync(item.ProductId);
                    int previousStock = product.Stock;

                    product.Stock -= item.Quantity;

                    bool updated = await _productService.UpdateAsync(product.Id, product);

                    var transaction = new InventoryTransaction
                    {
                        ProductId = product.Id,
                        TransactionType = InventoryTransactionType.StockOut,
                        Quantity = item.Quantity,
                        PreviousStock = previousStock,
                        NewStock = product.Stock,
                        ReferenceNo = null,
                        Notes = "Sold Out",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _inventoryService.AddAsync(transaction);
                    await _inventoryService.SaveChangesAsync();

                }
            }
            

            return RedirectToAction("OrderDetails", new { id = orderId });
        }
        public IActionResult Customers()
        {
            return View();
        }

        public IActionResult Orders()
        {
            _orderService.MakeUnseenOrdersSeen();
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

        public IActionResult AlertPartial()
        {
            return ViewComponent("AdminAlert");
        }

    }
}
