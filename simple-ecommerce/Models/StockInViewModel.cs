using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class StockInViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        [Required]
        [Range(1, 100000)]
        public int Quantity { get; set; }

        [StringLength(50)]
        public string? ReferenceNo { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
