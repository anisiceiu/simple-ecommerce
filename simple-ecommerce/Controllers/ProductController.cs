using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using simple_ecommerce.Models;

namespace simple_ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _CategoryService;
        public ProductController(IProductService productService, ICategoryService categoriesService)
        {
          _productService = productService;
            _CategoryService = categoriesService;
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
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetProductsAsync();    

            return Ok(products);
        }
    }
}
