using ECommerce.Domain;
using System.ComponentModel.DataAnnotations.Schema;
namespace ECommerce.Domain;
public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public string Status { get; set; } // Pending, Confirmed, Cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser User { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}
