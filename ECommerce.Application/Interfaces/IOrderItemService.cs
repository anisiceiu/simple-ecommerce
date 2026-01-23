using ECommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
        Task<OrderItem?> GetByIdAsync(int id);
        Task<OrderItem> CreateAsync(OrderItem item);
        Task<bool> UpdateAsync(int id, OrderItem item);
        Task<bool> DeleteAsync(int id);
    }

}
