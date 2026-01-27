using ECommerce.Domain;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class OrderRepository: IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task MakeUnseenOrdersSeen()
        {

            var unseenOrders = _context.Orders
            .Where(o => !o.IsSeenByAdmin)
            .ToList();

            unseenOrders.ForEach(o => o.IsSeenByAdmin = true);
            _context.SaveChanges();
        }
        public IQueryable<Order> GetOrdersIQueryable(string userId)
        {
            return _context.Orders
                .Include(o => o.User)                // Include related user
                .Where(o => o.UserId == userId)      // Filter by logged-in user
                .OrderByDescending(o => o.Id);
        }
        public async Task<IEnumerable<Order>> GetAllOrderForListAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.Id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .Include(o => o.ShippingAddress)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                 .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            _context.OrderAddress.Add(order.ShippingAddress);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
