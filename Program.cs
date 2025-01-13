using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealtyHub.Database;
using RealtyHub.Models;
using RealtyHub.Services.Email;

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
        builder.Services.AddIdentity<User, IdentityRole>() 
            .AddEntityFrameworkStores<RealtyHubDbContext>()
            .AddDefaultTokenProviders();
        
        //return url
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/auth/signin";
            options.AccessDeniedPath = "/auth/access-denied";
        });

        builder.Services.Configure<IdentityOptions>((options) =>
        {
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedAccount = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            options.User.RequireUniqueEmail = true;

            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
        });

        //Email service
        builder.Services.AddTransient<IEmailService, EmailService>();

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
