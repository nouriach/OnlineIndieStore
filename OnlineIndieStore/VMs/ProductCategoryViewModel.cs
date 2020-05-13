using OnlineIndieStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.VMs
{
    public class ProductCategoryViewModel
    {
        public Product Product { get; set; }
        public List<CategoryName> Category { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public Image Image { get; set; }

    }
}
