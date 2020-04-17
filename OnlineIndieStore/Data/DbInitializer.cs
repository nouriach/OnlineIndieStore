using OnlineIndieStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.Data
{
    public class DbInitializer
    {

        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.ProductCategories.Any())
            {
                return;
            }
            InitializeCategories(context);
            InitializeProducts(context);
            InitializeProductCategories(context);
        }

        public AppDbContext InitializeCategories(AppDbContext context)
        {
            var categories = new Category[]
            {
                new Category
                {
                    CategoryName = "PhoneCases"
                },
                new Category
                {
                    CategoryName = "Audio"
                },
                new Category
                {
                    CategoryName = "Notebooks"
                },
                new Category
                {
                    CategoryName = "Sleep"
                },
                new Category
                {
                    CategoryName = "WaterBottles"
                }
            };
            foreach (Category category in categories)
            {
                context.Categories.Add(category);
            }
            context.SaveChanges();
            return 
        }
        public static void InitializeProducts(AppDbContext context)
        {
            var products = new Product[]
            {
               new Product
               {
                   Name="Hargraft - Wild Case",
                   Price= 86,
                   Description="lorum ipsum",
                   ImageUrl = "https://placeholder.com/",
               },
               new Product
               {
                   Name="Might Vibe Spotify Player",
                   Price= 80,
                   Description="lorum ipsum",
                   ImageUrl = "https://placeholder.com/",
               },
               new Product
               {
                   Name="Totebook (2 Pack)",
                   Price= 20,
                   Description="lorum ipsum",
                   ImageUrl = "https://placeholder.com/",
               },
               new Product
               {
                   Name="Oura Smart Ring",
                   Price= 314,
                   Description="lorum ipsum",
                   ImageUrl = "https://placeholder.com/",
               },
               new Product
               {
                   Name="Klean Kanteen Insulated TKPro",
                   Price= 45,
                   Description="lorum ipsum",
                   ImageUrl = "https://placeholder.com/",
               }
            };
            foreach (Product product in products)
            {
                context.Products.Add(product);
            }
        }

        public static void InitializeProductCategories (AppDbContext context)
        {
            var productcategories = new ProductCategory[]
            {
                new ProductCategory { ProductID=1, CategoryID=1 },
                new ProductCategory { ProductID=2, CategoryID=2 },
                new ProductCategory { ProductID=3, CategoryID=3 },
                new ProductCategory { ProductID=4, CategoryID=4 },
                new ProductCategory { ProductID=5, CategoryID=5 },
            };
            foreach (ProductCategory pc in productcategories)
            {
                context.ProductCategories.Add(pc);
            }
            context.SaveChanges();
        }
    }
}
