using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    using ECommerce.Domain;
    using ECommerce.Domain.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _context.OrderItems
                .Include(x => x.Product)
                .Include(x => x.Order)
                .ToListAsync();
        }

        public async Task<OrderItem?> GetByIdAsync(int id)
        {
            return await _context.OrderItems
                .Include(x => x.Product)
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Include(x => x.Product)
                .Where(x => x.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<OrderItem> AddAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(OrderItem item)
        {
            _context.OrderItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.OrderItems.FindAsync(id);
            if (item == null) return;

            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

}
