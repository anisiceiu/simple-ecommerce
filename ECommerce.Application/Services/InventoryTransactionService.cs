using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly IInventoryTransactionRepository _repo;

        public InventoryTransactionService(IInventoryTransactionRepository repo)
        {
            _repo = repo;
        }
        public async Task AddAsync(InventoryTransaction transaction)
        {
            await _repo.AddAsync(transaction);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _repo.GetByDateRangeAsync(from, to);
        }

        public async Task<InventoryTransaction?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(int productId)
        {
            return await _repo.GetByProductIdAsync(productId);
        }

        public async Task<IEnumerable<InventoryTransaction>> GetRecentAsync(int count)
        {
            return await _repo.GetRecentAsync(count);
        }

        public async Task SaveChangesAsync()
        {
            await _repo.SaveChangesAsync();
        }
    }
}
