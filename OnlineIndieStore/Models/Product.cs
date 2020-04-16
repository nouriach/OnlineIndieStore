using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.Models
{
    public class Product
    {
        // Primary Key
        public int ID { get; set; }

        // Properties
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        // Foreign Key

        // Navigational Properties
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
