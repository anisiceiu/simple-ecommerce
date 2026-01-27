using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace simple_ecommerce.Controllers
{
    public class LanguageController : Controller
    {
        [HttpGet]
        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true
                });

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
