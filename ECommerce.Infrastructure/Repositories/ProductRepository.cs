using ECommerce.Domain;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace ECommerce.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;


    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<Product>> GetAllAsync()
    => await _context.Products.ToListAsync();//.Include(p=> p.Ratings)


    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)//.Include(o=>o.Ratings)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task RateProduct(string userId, int productId, int rating)
    {
        var existing = await _context.ProductRatings
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

        if (existing == null)
        {
            _context.ProductRatings.Add(new ProductRating
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating
            });
        }
        else
        {
            existing.Rating = rating;
        }

        await _context.SaveChangesAsync();
    }
}