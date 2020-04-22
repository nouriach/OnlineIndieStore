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

            var products = new Product[]
            {
               new Product
               {
                   ID = 0,
                   Name="Hargraft - Wild Case",
                   Price= 86,
                   Description="lorum ipsum",
                   ImageUrl = "https://via.placeholder.com/300",
               },
               new Product
               {
                   ID = 1,
                   Name="Might Vibe Spotify Player",
                   Price= 80,
                   Description="lorum ipsum",
                   ImageUrl = "https://via.placeholder.com/300",
               },
               new Product
               {
                   ID = 2,
                   Name="Totebook (2 Pack)",
                   Price= 20,
                   Description="lorum ipsum",
                   ImageUrl = "https://via.placeholder.com/300",
               },
               new Product
               {
                   ID = 3,
                   Name="Oura Smart Ring",
                   Price= 314,
                   Description="lorum ipsum",
                   ImageUrl = "https://via.placeholder.com/300",
               },
               new Product
               {
                   ID = 4,
                   Name="Klean Kanteen Insulated TKPro",
                   Price= 45,
                   Description="lorum ipsum",
                   ImageUrl = "https://via.placeholder.com/300",
               }
            };
            foreach (Product product in products)
            {
                context.Products.Add(product);
            }

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

            var productcategories = new ProductCategory[]
            {
                new ProductCategory { ProductID=0, CategoryID=0, Selection = Selection.Tech },
                new ProductCategory { ProductID=1, CategoryID=1, Selection = Selection.Audio },
                new ProductCategory { ProductID=2, CategoryID=2, Selection = Selection.Bottles },
                new ProductCategory { ProductID=3, CategoryID=3, Selection = Selection.Footwear },
                new ProductCategory { ProductID=4, CategoryID=4, Selection = Selection.Headware },
            };
            foreach (ProductCategory pc in productcategories)
            {
                context.ProductCategories.Add(pc);
            }
            context.SaveChanges();
        }
    }
}
