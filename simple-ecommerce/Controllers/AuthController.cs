using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using simple_ecommerce.Models;
using System.Security.Claims;

namespace simple_ecommerce.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateUserDto dto)
        {
            if (string.IsNullOrEmpty(dto.Phone))
                ModelState.AddModelError(nameof(dto.Phone), "Phone number is required");

            if (dto.Password != dto.ConfirmPassword)
                ModelState.AddModelError(nameof(dto.ConfirmPassword), "Passwords do not match");

            if (!ModelState.IsValid)
                return View(dto);

            var user = new ApplicationUser
            {
                UserName = dto.Phone,
                NormalizedUserName = (dto.FirstName + " " + dto.LastName).ToUpper(),
                Email = $"no_{dto.Phone}@test.com",
                PhoneNumber = dto.Phone,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await _authService.AddUserAsync(user, dto.Password);

            if (result == null)
            {
                ModelState.AddModelError("", "User registration failed");
                return View(dto);
            }

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto, string returnUrl = null)
        {
            var user = await _authService.ValidateUserAsync(dto.Phone, dto.Password);
            

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(dto);
            }

            var roles = await _authService.GetRolesAsync(user);
            var claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Name, user.PhoneNumber),
                                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                                    };

            // add role claims
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);


            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                    IdentityConstants.ApplicationScheme, // ✅ correct scheme
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = dto.RememberMe, //remember me
                        ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });

            // Check if returnUrl is valid and local to prevent open redirect attacks
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme); // use correct scheme
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();

        // GET
        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult Success()
        {
            return View();
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // refresh sign-in so user stays logged in
            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] = "Password changed successfully";
            return RedirectToAction(nameof(Success));
        }
    }
}
