using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineIndieStore.Models
{
    public class Category
    {
        // Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryID { get; set; }

        // Properties
        public string CategoryName { get; set; }

        // Foreign Key

        // Navigational Properties
        public ICollection<ProductCategory> ProductCategories { get; set; }


    }
}