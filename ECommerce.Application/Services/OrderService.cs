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
    public class OrderService: IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Order>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<Order?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public Task<Order> CreateAsync(Order order)
            => _repo.AddAsync(order);

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return false;

            order.Status = status;
            await _repo.UpdateAsync(order);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
