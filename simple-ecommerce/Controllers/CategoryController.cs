using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using ECommerce.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ecommerce.Controllers
{
    public class CategoryController : Controller
    {
        
        private readonly ICategoryService _repo;
        public CategoryController(ICategoryService _CategoryService)
        {

            _repo = _CategoryService;
        }


        public async Task<IActionResult> Index()
        {
            return await Task.Run(()=> View());
        }
        public IActionResult Create()
        {
            return View();
        }
        // GET: Categories
        public async Task<IActionResult> GetAll()
        {
            var data = (await _repo.GetAllAsync()).ToList();
            return Ok(data);
        }

       
         

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);
            var category = new Category
            {
                 Name = dto.Name,
                 CreatedAt = dto.CreatedAt,
                 IsActive = dto.IsActive,
            };
            await _repo.AddAsync(category);
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(category);

            var updated = await _repo.UpdateAsync(id, category);
            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // OPTIONAL: Details
        public async Task<IActionResult> Details(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}
