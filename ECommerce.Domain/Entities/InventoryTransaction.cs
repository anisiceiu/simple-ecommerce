using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class InventoryTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [Required]
        public InventoryTransactionType TransactionType { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int PreviousStock { get; set; }

        [Required]
        public int NewStock { get; set; }

        [MaxLength(50)]
        public string? ReferenceNo { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
