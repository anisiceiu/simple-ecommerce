using ECommerce.Application.DTOs;
using ECommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProductsAsync();
        Task CreateAsync(ProductDto dto);
        Task<Product?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);
        Task RateProduct(string userId, int productId, int rating);
    }
}
