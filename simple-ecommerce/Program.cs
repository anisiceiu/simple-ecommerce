using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ecommerce.Hubs;
using System.Globalization;
using System.Reflection;

namespace ecommerce
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

            #region Localization
            //Step 1
            builder.Services.AddSingleton<LanguageService>();
            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });


            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                        new CultureInfo("en"),
                        new CultureInfo("bn")
                    };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            });

            #endregion


            builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();


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

            //Step 2
            app.UseRequestLocalization(
                    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value
                );
            app.UseRouting();

            var assembly = typeof(SharedResource).Assembly;
            var names = assembly.GetManifestResourceNames();
            Console.WriteLine("Embedded resources in SharedResource assembly:");
            foreach (var name in names)
            {
                Console.WriteLine(name);
            }



            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/chatHub");
            app.MapHub<AdminNotificationHub>("/adminHub");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
