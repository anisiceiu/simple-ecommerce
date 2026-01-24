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

        public async Task<IEnumerable<Order>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<Order?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task<Order> CreateAsync(Order order)
            => await _repo.AddAsync(order);

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

        public async Task<IEnumerable<Order>> GetAllOrderForListAsync()
        {
            return await _repo.GetAllOrderForListAsync();
        }

        public IQueryable<Order> GetOrdersIQueryable(string userId)
        {
           return _repo.GetOrdersIQueryable(userId);
        }
    }
}
