using System.Diagnostics;
using ECommerce.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using simple_ecommerce.Models;

namespace simple_ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var products = new List<ProductDto> {
                new ProductDto{ Id = 1, Name="Product 1",Price=10},
                new ProductDto{ Id = 2, Name="Product 2",Price=15},
                new ProductDto{ Id = 3, Name="Product 3",Price=20},
                new ProductDto{ Id = 4, Name="Product 4",Price=25},
            };

            return View(products);
        }

        public IActionResult ProductDetails(int id)
        {
            var products = new List<ProductDto> {
                new ProductDto{ Id = 1, Name="Product 1",Price=10},
                new ProductDto{ Id = 2, Name="Product 2",Price=15},
                new ProductDto{ Id = 3, Name="Product 3",Price=20},
                new ProductDto{ Id = 4, Name="Product 4",Price=25},
            };

            var pro = products.FirstOrDefault(c=> c.Id == id);

            return View(pro);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
