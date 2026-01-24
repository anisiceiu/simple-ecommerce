using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class ProductRating
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }

        public int Rating { get; set; } // 1–5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; }
    }
}
