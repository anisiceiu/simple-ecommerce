using ECommerce.Application.DTOs;
using ECommerce.Domain;

namespace simple_ecommerce.Models
{
    public class ProductDetailsVM
    {
        public ProductDto Product { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int? UserRating { get; set; }
    }
}
