using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineIndieStore.Models
{
    public enum CategoryName
    {
        PhoneCases,
        Charging,
        PowerBanks,
        WirelessChargers,
        AppleAccesories,
        Keyboards,
        Audio,
        Notebooks,
        Stationery,
        Sleep,
        WaterBottles,
        LunchBoxes,
        Clocks,
        Watches,
        Wallets,
        Kitchenware,
        Backpacks,
        Headwear,
        Footwear
    }
    public class Category
    {
        // Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryID { get; set; }

        // Properties
        public CategoryName? CategoryName { get; set; }
        public bool IsChecked { get; set; }

        // Foreign Key

        // Navigational Properties
        public ICollection<ProductCategory> ProductCategories { get; set; }


    }
}