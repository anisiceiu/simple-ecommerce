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
    => await _context.Products.Include(p => p.Category).ToListAsync();//.Include(p=> p.Ratings)

    public async Task<Dictionary<int, (double AverageRating, int TotalRatings)>> GetProductRatingsAsync(List<int> productIds)
    {
        var ratings = await _context.ProductRatings
            .Where(r => productIds.Contains(r.ProductId))
            .GroupBy(r => r.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                AverageRating = g.Any() ? Math.Round(g.Average(r => r.Rating), 1) : 0.0,
                TotalRatings = g.Count()
            })
            .ToListAsync();

        return ratings.ToDictionary(x => x.ProductId, x => (x.AverageRating, x.TotalRatings));
    }


    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category).Include(o => o.Ratings)
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
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Rating = rating;
            existing.CreatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<ProductRating>> GetProductRatingsAsync(int productId)
    {
        return await _context.ProductRatings
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<double> GetAverageRatingAsync(int productId)
    {
        var ratings = await _context.ProductRatings
            .Where(r => r.ProductId == productId)
            .ToListAsync();

        if (ratings.Count == 0) return 0;
        return Math.Round(ratings.Average(r => r.Rating), 1);
    }

    public async Task<Dictionary<int, int>> GetRatingDistributionAsync(int productId)
    {
        var ratings = await _context.ProductRatings
            .Where(r => r.ProductId == productId)
            .GroupBy(r => r.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync();

        var distribution = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };
        foreach (var item in ratings)
        {
            distribution[item.Rating] = item.Count;
        }

        return distribution;
    }

    public async Task<int?> GetUserRatingAsync(int productId, string userId)
    {
        var rating = await _context.ProductRatings
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

        return rating?.Rating;
    }

    public async Task<List<ProductRating>> GetAllRatingsAsync()
    {
        return await _context.ProductRatings
            .Include(r => r.Product)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}