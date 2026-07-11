using ECommerce.Domain.Entities;
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
        Task<List<ProductRating>> GetProductRatingsAsync(int productId);
        Task<double> GetAverageRatingAsync(int productId);
        Task<Dictionary<int, int>> GetRatingDistributionAsync(int productId);
        Task<int?> GetUserRatingAsync(int productId, string userId);
        Task<List<ProductRating>> GetAllRatingsAsync();
        Task<Dictionary<int, (double AverageRating, int TotalRatings)>> GetProductRatingsAsync(List<int> productIds);
    }
}
