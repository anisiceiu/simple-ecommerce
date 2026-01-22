using ECommerce.Domain;
using System.ComponentModel.DataAnnotations;
namespace ECommerce.Domain;
public class Cart
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser User { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}
