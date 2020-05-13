using OnlineIndieStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.VMs
{
    public class DisplayProductViewModel
    {
        public Product Product { get; set; }
        public List<Category> Categories { get; set; }
        public string Selection { get; set; }
        public Image Image { get; set; }

    }
}
