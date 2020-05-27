using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.Models
{
    public class Item
    {
        //Primary Key
        public int ID { get; set; }

        // Properties
        public int Quantity { get; set; }

        // Navigational Property
        public Product Product { get; set; }

        // Foreign Key
        public int ProductId { get; set; }
    }
}
