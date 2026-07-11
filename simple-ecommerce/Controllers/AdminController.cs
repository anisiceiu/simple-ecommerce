using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;

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

        public async Task<IActionResult> GetAllOrders(string? startDate = null, string? endDate = null)
        {
            DateTime? startDateTime = null;
            DateTime? endDateTime = null;

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var start))
            {
                startDateTime = start;
            }

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
            {
                endDateTime = end;
            }

            IEnumerable<Order> orders;
            if (startDateTime.HasValue || endDateTime.HasValue)
            {
                orders = await _orderService.GetOrdersByDateRangeAsync(startDateTime, endDateTime);
            }
            else
            {
                orders = await _orderService.GetAllOrderForListAsync();
            }

            return Ok(orders);
        }

        public IActionResult AlertPartial()
        {
            return ViewComponent("AdminAlert");
        }

        public async Task<IActionResult> ExportOrdersToExcel(string? startDate = null, string? endDate = null)
        {
            try
            {
                DateTime? startDateTime = null;
                DateTime? endDateTime = null;

                if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var start))
                {
                    startDateTime = start;
                }

                if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
                {
                    endDateTime = end;
                }

                IEnumerable<Order> orders;
                if (startDateTime.HasValue || endDateTime.HasValue)
                {
                    orders = await _orderService.GetOrdersByDateRangeAsync(startDateTime, endDateTime);
                }
                else
                {
                    orders = await _orderService.GetAllOrderForListAsync();
                }

                // Create Excel workbook
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Orders");

                    // Add headers
                    worksheet.Cell(1, 1).Value = "Order ID";
                    worksheet.Cell(1, 2).Value = "Customer Phone";
                    worksheet.Cell(1, 3).Value = "Total Amount";
                    worksheet.Cell(1, 4).Value = "Status";
                    worksheet.Cell(1, 5).Value = "Created Date";

                    // Format header row
                    var headerRow = worksheet.Range(1, 1, 1, 5);
                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Add data rows
                    int row = 2;
                    foreach (var order in orders)
                    {
                        worksheet.Cell(row, 1).Value = order.Id;
                        worksheet.Cell(row, 2).Value = order.User?.PhoneNumber ?? "N/A";
                        worksheet.Cell(row, 3).Value = order.TotalAmount;
                        worksheet.Cell(row, 4).Value = order.Status;
                        worksheet.Cell(row, 5).Value = order.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                        row++;
                    }

                    // Adjust column widths
                    worksheet.Column(1).Width = 12;
                    worksheet.Column(2).Width = 18;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 20;

                    // Format currency column
                    worksheet.Column(3).Style.NumberFormat.Format = "$#,##0.00";

                    // Save to memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        // Generate filename with date range
                        string fileName;
                        if (startDateTime.HasValue && endDateTime.HasValue)
                        {
                            fileName = $"Orders_{startDateTime:yyyy-MM-dd}_to_{endDateTime:yyyy-MM-dd}.xlsx";
                        }
                        else if (startDateTime.HasValue)
                        {
                            fileName = $"Orders_from_{startDateTime:yyyy-MM-dd}.xlsx";
                        }
                        else if (endDateTime.HasValue)
                        {
                            fileName = $"Orders_until_{endDateTime:yyyy-MM-dd}.xlsx";
                        }
                        else
                        {
                            fileName = $"Orders_{DateTime.Now:yyyy-MM-dd_HHmmss}.xlsx";
                        }

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error exporting orders", error = ex.Message });
            }
        }

        public async Task<IActionResult> Ratings()
        {
            var ratings = await _productService.GetAllRatingsAsync();

            var statistics = new
            {
                TotalRatings = ratings.Count(),
                AverageRating = ratings.Any() ? Math.Round(ratings.Average(r => r.Rating), 2) : 0,
                FiveStarRatings = ratings.Count(r => r.Rating == 5),
                FourStarRatings = ratings.Count(r => r.Rating == 4),
                ThreeStarRatings = ratings.Count(r => r.Rating == 3),
                TwoStarRatings = ratings.Count(r => r.Rating == 2),
                OneStarRatings = ratings.Count(r => r.Rating == 1),
                RatingsWithComments = ratings.Count(r => !string.IsNullOrEmpty(r.Comment))
            };

            ViewBag.Statistics = statistics;
            return View();
        }

        public async Task<IActionResult> GetAllRatings()
        {
            var ratings = await _productService.GetAllRatingsAsync();
            var ratingsData = ratings.Select(r => new
            {
                r.Id,
                r.ProductId,
                ProductName = r.Product?.Name ?? "Unknown Product",
                r.UserId,
                r.Rating,
                r.Comment,
                CreatedAt = r.CreatedAt.ToString("MMM dd, yyyy HH:mm")
            }).ToList();

            return Ok(ratingsData);
        }

    }
}

