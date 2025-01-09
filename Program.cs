using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealtyHub.Database;
using RealtyHub.Models;

namespace RealtyHub;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //==============================================================================
        // Add services to the container.
        //==============================================================================

        builder.Services.AddControllersWithViews();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddDbContext<RealtyHubDbContext>(options =>
        {
            options.UseSqlServer( builder.Configuration.GetConnectionString("DefaultConnection") );
        });

        //Setup Identity: User -> IdentityUser
        builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
            // options.Password.RequireDigit = true;
            // options.Password.RequireLowercase = true;
            // options.Password.RequireUppercase = true;
            // options.Password.RequireNonAlphanumeric = true;
            // options.Password.RequiredLength = 8;
            // options.SignIn.RequireConfirmedEmail = true;
        })  
        .AddEntityFrameworkStores<RealtyHubDbContext>() // Add Identity to the project
        .AddDefaultTokenProviders(); // Add token provider for password reset
        
        //return url
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/auth/signin";
            options.AccessDeniedPath = "/auth/accessdenied";
        });

        var app = builder.Build();

        //==============================================================================
        // Configure the HTTP request pipeline.
        //==============================================================================
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication(); // identity
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
