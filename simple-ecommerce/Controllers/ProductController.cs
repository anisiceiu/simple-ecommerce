using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using ECommerce.Domain.Entities;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using simple_ecommerce.Models;
using System.Security.Claims;

namespace simple_ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _CategoryService;
        private readonly IWebHostEnvironment _env;
        public ProductController(IProductService productService, ICategoryService categoriesService, IWebHostEnvironment env)
        {
          _productService = productService;
            _CategoryService = categoriesService;
            _env = env;
        }

        public async Task<IActionResult> Create()
        {
            var vm = new ProductCreateVM
            {
                Product = new ProductDto(),
                Categories = (await _CategoryService.GetAllAsync()).Where(c => c.IsActive)
                            .Select(c => new SelectListItem
                            {
                                Value = c.Id.ToString(),
                                Text = c.Name
                            }).ToList()

            };

            return View(vm);
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (ModelState.IsValid)
            {
                var imageUrl = await SaveImage(vm.Product.Image);

                vm.Product.ImageUrl = imageUrl;

                vm.Product.CreatedAt = DateTime.Now;

                await _productService.CreateAsync(vm.Product);

                TempData["success"] = "Product added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdown if validation fails
            vm.Categories = (await _CategoryService.GetAllAsync()).Where(c => c.IsActive)
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.Id.ToString(),
                                      Text = c.Name
                                  }).ToList();

            return View(vm);
        }
        private async Task<string> SaveImage(IFormFile file)
        {
            if (file == null) return string.Empty;

            var uploads = Path.Combine(_env.WebRootPath, "images/Products");
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"{fileName}";
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetProductsAsync();    

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Rate(int productId, int rating)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _productService.RateProduct(userId,productId,rating);

            return Ok();
        }
    }
}
