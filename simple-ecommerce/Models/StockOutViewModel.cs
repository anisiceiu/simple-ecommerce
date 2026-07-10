using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class StockOutViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        [Required]
        [Range(1, 100000)]
        public int Quantity { get; set; }

        public string? ReferenceNo { get; set; }

        public string? Notes { get; set; }
    }
}
