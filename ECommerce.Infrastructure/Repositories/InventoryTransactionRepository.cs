using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    using ECommerce.Domain.Entities;
    using ECommerce.Domain.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly AppDbContext _context;

        public InventoryTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InventoryTransaction transaction)
        {
            await _context.InventoryTransactions.AddAsync(transaction);
        }

        public async Task<InventoryTransaction?> GetByIdAsync(int id)
        {
            return await _context.InventoryTransactions
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(int productId)
        {
            return await _context.InventoryTransactions
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(
            DateTime from,
            DateTime to)
        {
            return await _context.InventoryTransactions
                .Where(x => x.CreatedAt >= from && x.CreatedAt <= to)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetRecentAsync(int count)
        {
            return await _context.InventoryTransactions
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
