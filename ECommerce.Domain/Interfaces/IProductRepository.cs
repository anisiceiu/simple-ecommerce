using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task AddAsync(Product product);

        Task<Product?> GetByIdAsync(int id);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task RateProduct(string userId, int productId, int rating);
    }
}
