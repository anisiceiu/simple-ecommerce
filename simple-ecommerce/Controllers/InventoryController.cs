using ecommerce.Models;
using ECommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IProductRepository _productRepository;
        public InventoryController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<IActionResult> Index(string? search)
        {
            var products = await _productRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = (List<ECommerce.Domain.Product>)products.Where(x =>
                    x.Name.Contains(search,
                        StringComparison.OrdinalIgnoreCase));
            }

            var productList=products.Select(p =>
            new ProductViewModel
            {
                Category = p.Category,
                CategoryId = p.CategoryId,
                CreatedAt = p.CreatedAt,
                Description = p.Description,
                Id = p.Id,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                Name = p.Name,
                Price = p.Price,
                Ratings = p.Ratings,
                ReorderLevel = p.ReorderLevel,
                Stock = p.Stock
            }).ToList();

            return View(productList);
        }
    }
}
