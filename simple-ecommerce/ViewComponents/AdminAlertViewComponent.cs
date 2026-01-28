using ECommerce.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using ecommerce.Models;

namespace ecommerce.ViewComponents
{
    public class AdminAlertViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public AdminAlertViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var newOrders = _context.Orders
                .Where(o => !o.IsSeenByAdmin)
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .Select(o => new OrderAlertItem
                {
                    OrderId = o.Id,
                    Date = o.CreatedAt,
                    Total = o.TotalAmount
                })
                .ToList();

            return View(new AdminAlertVM
            {
                NewOrderCount = newOrders.Count,
                Orders = newOrders
            });
        }
    }

}
