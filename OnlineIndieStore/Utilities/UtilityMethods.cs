using OnlineIndieStore.Data;
using OnlineIndieStore.Models;
using OnlineIndieStore.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

       public static List<DisplayProductViewModel> GetAllLiveProducts(List<Product> db)
        {
            List<DisplayProductViewModel> displayProds = new List<DisplayProductViewModel>();

            foreach (var item in db)
            {
                // instantiate View Model
                DisplayProductViewModel displayPvm = new DisplayProductViewModel();
                // Create list of Categories to store incoming Categories
                List<Category> associatedCategories = new List<Category>();

                // Set new View Model Product to selected Product in the database
                displayPvm.Product = item;

                // For each entry in the ProductCategory table see where the ProductID matches the selected Product ID and store the Selection value
                var selection = item.ProductCategories.Where(x => x.ProductID == item.ID).Select(y => y.Selection).FirstOrDefault();
                displayPvm.Selection = selection.ToString();

                // For each Categories with this database Product loop through all the assigned Categories and add them
                foreach (var t in item.ProductCategories)
                {
                    associatedCategories.Add(t.Category);
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
