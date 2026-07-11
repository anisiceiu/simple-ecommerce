using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;


        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }


        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var products = await _repo.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                ProductCode = p.ProductCode,
                CategoryId = p.CategoryId,
                CreatedAt = p.CreatedAt,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                ReorderLevel = p.ReorderLevel
                //Ratings = p.Ratings.ToList()
            }).ToList();
        }


        public async Task CreateAsync(ProductDto dto)
        {
            // Basic validation to avoid DB constraint errors
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name)) dto.Name = string.Empty;

            // Generate product code in format PRD0001, PRD0002, etc.
            var allProducts = await _repo.GetAllAsync();
            var nextProductNumber = (allProducts.Count > 0 ? allProducts.Max(p => p.Id) : 0) + 1;
            var productCode = $"PRD{nextProductNumber:D4}";

            var product = new Product()
            {
                ProductCode = productCode,
                CategoryId = dto.CategoryId,
                Description = dto.Description ?? string.Empty,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                IsActive = dto.IsActive,
                Name = dto.Name ?? string.Empty,
                Price = dto.Price,
                Stock = dto.Stock,
                ReorderLevel = dto.ReorderLevel,
                CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt

            };
            await _repo.AddAsync(product);
        }
        public Task<Product?> GetByIdAsync(int id)
       => _repo.GetByIdAsync(id);

        public async Task<bool> UpdateAsync(int id, Product product)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            existing.ImageUrl = product.ImageUrl;
            existing.IsActive = product.IsActive;
            existing.CategoryId = product.CategoryId;

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

        public async Task RateProduct(string userId, int productId, int rating)
        {
           await _repo.RateProduct(userId,productId,rating);
        }

        public async Task<List<ProductRating>> GetProductRatingsAsync(int productId)
        {
            return await _repo.GetProductRatingsAsync(productId);
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            return await _repo.GetAverageRatingAsync(productId);
        }

        public async Task<Dictionary<int, int>> GetRatingDistributionAsync(int productId)
        {
            return await _repo.GetRatingDistributionAsync(productId);
        }

        public async Task<int?> GetUserRatingAsync(int productId, string userId)
        {
            return await _repo.GetUserRatingAsync(productId, userId);
        }

        public async Task<List<ProductRating>> GetAllRatingsAsync()
        {
            return await _repo.GetAllRatingsAsync();
        }

        public async Task<Dictionary<int, (double AverageRating, int TotalRatings)>> GetProductRatingsForListAsync(List<int> productIds)
        {
            return await _repo.GetProductRatingsAsync(productIds);
        }
    }

}
