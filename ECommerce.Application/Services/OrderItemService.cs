using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using ECommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _repo;

        public OrderItemService(IOrderItemRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<OrderItem>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
            => _repo.GetByOrderIdAsync(orderId);

        public Task<OrderItem?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public Task<OrderItem> CreateAsync(OrderItem item)
            => _repo.AddAsync(item);

        public async Task<bool> UpdateAsync(int id, OrderItem item)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.ProductId = item.ProductId;
            existing.Quantity = item.Quantity;
            existing.UnitPrice = item.UnitPrice;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }
    }

}
