using ECommerce.Domain;
using System.ComponentModel.DataAnnotations.Schema;
namespace ECommerce.Domain;
public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    // Navigation
    public Cart Cart { get; set; }
    public Product Product { get; set; }
}
