using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Interfaces
{
    public interface IInventoryTransactionRepository
    {
        Task AddAsync(InventoryTransaction transaction);

        Task<InventoryTransaction?> GetByIdAsync(int id);

        Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(int productId);

        Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(
            DateTime from,
            DateTime to);

        Task<IEnumerable<InventoryTransaction>> GetRecentAsync(int count);

        Task SaveChangesAsync();
    }
}
