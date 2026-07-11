using ECommerce.Application.DTOs;
using ECommerce.Domain;
using ECommerce.Domain.Entities;

namespace ecommerce.Models
{
    public class ProductDetailsVM
    {
        public ProductDto Product { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int? UserRating { get; set; }
        public List<ProductRatingVM> Reviews { get; set; } = new List<ProductRatingVM>();
        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();
    }

    public class ProductRatingVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
