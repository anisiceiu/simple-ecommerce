using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using simple_ecommerce.Hubs;

namespace simple_ecommerce
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                 b => b.MigrationsAssembly("ECommerce.Infrastructure")
                ));

            // 2️⃣ Add Identity WITH roles
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });

            builder.Services.AddHttpContextAccessor();
            #region DI
            builder.Services.AddInfrastructure();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();
            


            #endregion

            // Configure cookie paths
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/auth/Login";      // <-- your custom login URL
                options.AccessDeniedPath = "/auth/AccessDenied"; // optional
                options.LogoutPath = "/auth/Logout";    // optional
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;   // for dev
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; // for dev
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;

                options.Events = new CookieAuthenticationEvents
                {
                    OnSignedIn = context =>
                    {
                        Console.WriteLine("User signed in successfully");
                        return Task.CompletedTask;
                    },
                    OnRedirectToLogin = context =>
                    {
                        Console.WriteLine($"Redirecting to login from: {context.Request.Path}");
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddSignalR();

            var app = builder.Build();
            // 🔹 Seed Identity data
            await IdentitySeeder.SeedRolesAndUsersAsync(app.Services);
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapHub<AdminNotificationHub>("/adminHub");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
