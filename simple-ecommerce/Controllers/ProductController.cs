using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using ECommerce.Domain.Entities;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ecommerce.Models;
using System.Security.Claims;

namespace ecommerce.Controllers
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

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var vm = new ProductCreateVM
            {
                Product = new ProductDto
                {
                    Id = product.Id,
                    CategoryId = product.CategoryId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ImageUrl = product.ImageUrl,
                    IsActive = product.IsActive,
                    ReorderLevel = product.ReorderLevel,
                    CreatedAt = product.CreatedAt
                },
                Categories = (await _CategoryService.GetAllAsync()).Where(c => c.IsActive)
                            .Select(c => new SelectListItem
                            {
                                Value = c.Id.ToString(),
                                Text = c.Name,
                                Selected = c.Id == product.CategoryId
                            }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductCreateVM vm)
        {
            if (ModelState.IsValid)
            {
                var product = await _productService.GetByIdAsync(vm.Product.Id);

                if (product == null)
                {
                    return NotFound();
                }

                // Handle image update
                if (vm.Product.Image != null && vm.Product.Image.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_env.WebRootPath, "images/Products", product.ImageUrl);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save new image
                    var imageUrl = await SaveImage(vm.Product.Image);
                    product.ImageUrl = imageUrl;
                }

                // Update product properties
                product.CategoryId = vm.Product.CategoryId;
                product.Name = vm.Product.Name;
                product.Description = vm.Product.Description;
                product.Price = vm.Product.Price;
                product.Stock = vm.Product.Stock;
                product.IsActive = vm.Product.IsActive;
                product.ReorderLevel = vm.Product.ReorderLevel;

                await _productService.UpdateAsync(product.Id, product);

                TempData["success"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdown if validation fails
            vm.Categories = (await _CategoryService.GetAllAsync()).Where(c => c.IsActive)
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.Id.ToString(),
                                      Text = c.Name,
                                      Selected = c.Id == vm.Product.CategoryId
                                  }).ToList();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                // Delete associated image
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(_env.WebRootPath, "images/Products", product.ImageUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                await _productService.DeleteAsync(id);

                return Json(new { success = true, message = "Product deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting product: {ex.Message}" });
            }
        }

    }
}
