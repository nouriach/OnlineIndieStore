using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.Models
{
    public enum Selection
    {
        Tech,
        Audio,
        Stationery,
        Sleep,
        Bottles,
        LunchBoxes,
        Time,
        Wallets,
        Homeware,
        Luggage,
        Headware,
        Footwear
    }
    public class ProductCategory
    {
        // Primary Key
        public int ProductCategoryID { get; set; }

        // Foregin Key

        public int ProductID { get; set; }
        public int CategoryID { get; set; }

        // Property (Payload)

        public Selection? Selection { get; set; }

        // Navigational Properties

        public Product Product { get; set; }
        public Category Category { get; set; }

    }
}
