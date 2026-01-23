using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.DTOs; 
using ECommerce.Domain;


namespace ECommerce.Application.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly ICategoryRepository _repo;


        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        => _repo.GetAllAsync();

        public Task<Category?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public Task<Category> AddAsync(Category item)
            => _repo.AddAsync(item);

        public async Task<bool> UpdateAsync(int id, Category item)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Name = item.Name;
            existing.IsActive = item.IsActive;

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

    }
}
