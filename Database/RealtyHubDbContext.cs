
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealtyHub.Models;

namespace RealtyHub.Database
{
    public class RealtyHubDbContext : IdentityDbContext<User>
    {
        public RealtyHubDbContext(DbContextOptions<RealtyHubDbContext> options) 
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        //=========================================
        // This is where you define your tables
        //=========================================
        public DbSet<User> Users { get; set; }
        
        // public DbSet<Price> Prices { get; set; }
        // public DbSet<Property> Properties { get; set; }
        // public DbSet<Category> Categories { get; set; }

    }
}