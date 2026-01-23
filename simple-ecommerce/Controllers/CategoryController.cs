using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using ECommerce.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace simple_ecommerce.Controllers
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
            return View();
        }
        // GET: Categories
        public async Task<IActionResult> GetAll()
        {
            var data = (await _repo.GetAllAsync()).ToList();
            return Ok(data);
        }

       
         
        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

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
