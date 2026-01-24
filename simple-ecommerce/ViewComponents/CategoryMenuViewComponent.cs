namespace simple_ecommerce.ViewComponents
{
    using ECommerce.Application.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoryMenuViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = (await _categoryService.GetAllAsync())
                                .Where(c => c.IsActive)
                                .ToList();

            return View(categories);
        }
    }

}
