using Microsoft.EntityFrameworkCore;
using OnlineIndieStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineIndieStore.ViewModels;

namespace OnlineIndieStore.Data
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");
            modelBuilder.Entity<Product>().ToTable("Product");
        }

        public DbSet<OnlineIndieStore.ViewModels.SubmitProductViewModel> SubmitProductViewModel { get; set; }
    }
}
