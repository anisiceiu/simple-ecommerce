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
    public class CartService : ICartService
    {
        private readonly ICartRepository _repo;

        public CartService(ICartRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Cart>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<Cart?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _repo.GetByUserIdAsync(userId);
            if (cart != null) return cart;

            cart = new Cart
            {
                UserId = userId
            };

            return await _repo.AddAsync(cart);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cart = await _repo.GetByIdAsync(id);
            if (cart == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }
    }

}
