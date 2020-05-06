using OnlineIndieStore.Models;
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
    }
}
