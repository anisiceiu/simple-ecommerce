using ECommerce.Application.DTOs;
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
                CategoryId = p.CategoryId,
                CreatedAt = p.CreatedAt,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();
        }


        public async Task CreateAsync(ProductDto dto)
        {
            var product = new Product()
            {
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive,
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock

            };
            await _repo.AddAsync(product);
        }
    }
}
