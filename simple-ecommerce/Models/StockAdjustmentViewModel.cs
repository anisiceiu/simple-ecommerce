using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class StockAdjustmentViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        // Final stock after adjustment
        [Required]
        [Range(0, 100000)]
        public int NewStock { get; set; }

        public string? Notes { get; set; }
    }
}
