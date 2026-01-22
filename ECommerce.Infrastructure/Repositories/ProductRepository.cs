using ECommerce.Domain;
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
    => await _context.Products.ToListAsync();


    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
}