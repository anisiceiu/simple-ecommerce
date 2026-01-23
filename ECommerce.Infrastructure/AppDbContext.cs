namespace ECommerce.Infrastructure;

using ECommerce.Domain;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<OrderAddress> OrderAddress { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId);

        builder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId);

        builder.Entity<Address>()
            .HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId);

        builder.Entity<ApplicationUser>()
       .HasIndex(u => u.PhoneNumber)
       .IsUnique(); // enforce unique phone number

        // One-to-One: Order -> OrderAddress
        builder.Entity<Order>()
            .HasOne(o => o.ShippingAddress)
            .WithOne(a => a.Order)
            .HasForeignKey<OrderAddress>(a => a.OrderId)
            .OnDelete(DeleteBehavior.Cascade); // safe: deleting order deletes its address
    }
}
