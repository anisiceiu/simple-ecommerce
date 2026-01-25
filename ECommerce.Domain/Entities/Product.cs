using ECommerce.Domain;
using ECommerce.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ECommerce.Domain;
public class Product
{
    public int Id { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; }

    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    [MaxLength(255)]
    public string ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Category Category { get; set; }
    public ICollection<ProductRating>? Ratings { get; set; }
}
