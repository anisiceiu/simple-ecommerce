using ECommerce.Application.DTOs;
using ECommerce.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace simple_ecommerce.Models
{
    public class ProductCreateVM
    {
        public ProductDto Product { get; set; }


        public IEnumerable<SelectListItem> Categories { get; set; }=new List<SelectListItem>();
    }
}
