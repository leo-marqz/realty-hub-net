
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealtyHub.Models;

namespace RealtyHub.Database
{
    public class RealtyHubDbContext : IdentityDbContext
    {
        public RealtyHubDbContext(DbContextOptions options) 
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
        // public DbSet<Price> Prices { get; set; }
        // public DbSet<Property> Properties { get; set; }
        // public DbSet<Category> Categories { get; set; }

    }
}