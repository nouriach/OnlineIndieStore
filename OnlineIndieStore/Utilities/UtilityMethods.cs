using OnlineIndieStore.Data;
using OnlineIndieStore.Models;
using OnlineIndieStore.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing ;
using System.Threading.Tasks;
using ImageResizer;

namespace OnlineIndieStore.Utilities
{
    public class UtilityMethods
    {
        public static List<string> GetCategoryEnumsAsList()
        {
            List<string> categories = Enum.GetNames(typeof(CategoryName)).OrderBy(x => x).ToList();

            return categories;
        }

        public static List<string> GetSelectionEnumsAsList()
        {
            List<string> selections = Enum.GetNames(typeof(Selection)).OrderBy(y => y).ToList();

            return selections;
        }

        public static List<string> GetAllSelectionsInUse(AppDbContext appDbContext)
        {
            List<string> allExisitingSelections = new List<string>();
            foreach (var s in appDbContext.ProductCategories)
            {
                allExisitingSelections.Add(s.Selection.ToString());
            }

            return allExisitingSelections;
        }

        // Does the latest Image appear in this argumenmt?
       public static List<DisplayProductViewModel> GetAllLiveProducts(AppDbContext db)
        {
            List<DisplayProductViewModel> displayProds = new List<DisplayProductViewModel>();

            var allProducts = db.Products;

            foreach (var item in allProducts)
            {
                // instantiate View Model
                DisplayProductViewModel displayPvm = new DisplayProductViewModel();

                // Create list of Categories to store incoming Categories
                List<Category> associatedCategories = new List<Category>();

                // Set new View Model Product to selected Product in the database
                displayPvm.Product = item;

                // Set new View Model Image to selected Product's related Image in the database
                var img = db.Images.Where(x => x.ProductID == item.ID).FirstOrDefault();

                displayPvm.Image = img;

                // For each entry in the ProductCategory table see where the ProductID matches the selected Product ID and store the Selection value
                var selection = db.ProductCategories.Where(x => x.ProductID == item.ID).Select(y => y.Selection).FirstOrDefault();
                displayPvm.Selection = selection.ToString();


                // For each Categories with this database Product loop through all the assigned Categories and add them
                foreach (var t in db.ProductCategories)
                {
                    if (t.ProductID == item.ID)
                    {
                        var cat = db.Categories.Where(x => x.CategoryID == t.CategoryID).FirstOrDefault();
                        associatedCategories.Add(cat);
                    }
                }

                // Add all the Categories to the ViewModel Category
                displayPvm.Categories = associatedCategories;

                // Add new View Model to the ViewModel list
                displayProds.Add(displayPvm);
            }
            return displayProds;
        }
    }
}
