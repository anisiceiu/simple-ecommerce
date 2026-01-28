namespace ecommerce.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using ecommerce.Models;
    using System.Text.Json;

    namespace MyShop.Web.ViewComponents
    {
        public class CartSummaryViewComponent : ViewComponent
        {
            public IViewComponentResult Invoke()
            {
                var cartJson = HttpContext.Session.GetString("CART");

                var cart = string.IsNullOrEmpty(cartJson)
                    ? new List<CartItemViewModel>()
                    : JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson);

                var totalItems = cart.Sum(x => x.Quantity);
                var totalAmount = cart.Sum(x => x.Price * x.Quantity);

                return View(new CartSummaryViewModel
                {
                    Count = totalItems,
                    Total = totalAmount
                });
            }
        }
    }

}
