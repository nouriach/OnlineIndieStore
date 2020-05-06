using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineIndieStore.Data;
using OnlineIndieStore.Models;
using OnlineIndieStore.VMs;

namespace OnlineIndieStore.Controllers
{
    public class ProductCategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public ProductCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ProductCategories
        public async Task<IActionResult> Index(string? order, string? categoryOrder)
        {
            var appDbContext = _context.Products
                .Include(pc => pc.ProductCategories)
                    .ThenInclude(c => c.Category)
                .AsNoTracking();

            List<string> categories = Enum.GetNames(typeof(CategoryName)).OrderBy(x => x).ToList();
            ViewBag.CatOptions = categories;

            List<string> selections = Enum.GetNames(typeof(Selection)).OrderBy(y => y).ToList();
            ViewBag.SelOptions = selections;

            List<DisplayProductViewModel> displayProds = new List<DisplayProductViewModel>();

            foreach (var item in appDbContext)
            {
                // instantiate View Model
                DisplayProductViewModel displayPvm = new DisplayProductViewModel();
                // Create list of Categories to store incoming Categories
                List<Category> associatedCategories = new List<Category>();

                // Set new View Model Product to selected Product in the database
                displayPvm.Product = item;

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

            if (categoryOrder != null)
            {
                var returnFilteredCategories = FilterCategory(categoryOrder);
                return View(returnFilteredCategories);
            }

            switch (order)
            {
            case "ByPriceAscending":
                    return View(
                        displayProds.OrderBy(x => x.Product.Price).ToList());
            case "ByPriceDescending":
                return View(
                    displayProds
                    .OrderByDescending(x => x.Product.Price)
                    .ToList()
                    );
            case "ByNameDescending":
                return View(
                     displayProds
                    .OrderByDescending(x => x.Product.Name)
                    .ToList()
                    );
            default:
                return View(displayProds.OrderBy(x=> x.Product.Name).ToList());
            }
        }

        public List<DisplayProductViewModel> FilterCategory(string categoryOrder)
        {
            var appDbContext = _context.ProductCategories
                 .Include(p => p.Product)
                 .Include(c => c.Category)
                .AsNoTracking();

            try
            {
                // This is a method that takes the Enum value of Category and returns the CategoryID
                int newCategoryIndex = FindCategoryIndexInTable(categoryOrder);
                List<DisplayProductViewModel> displayProds = new List<DisplayProductViewModel>();
                List<Category> catList = new List<Category>();

                var filteredList = appDbContext.Where(x => x.CategoryID == newCategoryIndex).ToList();

                foreach(var i in filteredList)
                {
                    DisplayProductViewModel dp = new DisplayProductViewModel();
                    dp.Product = i.Product;

                    dp.Categories = appDbContext
                        .Where(x => x.ProductID == i.ProductID)
                        .Select(x => x.Category)
                        .ToList();

                    displayProds.Add(dp);
                }

                return displayProds.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private int FindCategoryIndexInTable(string categoryOrder)
        {
            try
            {
                // list all Categories
                var allCategoriesInTable = from categ in _context.Categories
                                           select categ;

                // Store the CategoryID that matches the Enum value
                int catIndex = 0;

                foreach (var enumCat in allCategoriesInTable)
                {
                    if (enumCat.CategoryName.ToString() == categoryOrder)
                    {
                        catIndex = enumCat.CategoryID;
                    }
                }

                return catIndex;
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        // GET: ProductCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories
                .Include(p => p.Category)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductCategoryID == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // GET: ProductCategories/Create
        public IActionResult Create()
        {
            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID");
            //ViewData["ProductID"] = new SelectList(_context.Products, "ID", "ID");

            ViewData["CategoryName"] = new SelectList(_context.Categories, "CategoryName", "CategoryName");
            ViewData["ProductName"] = new SelectList(_context.Products, "Name", "Name");

            return View();
        }

        // POST: ProductCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductCategoryID,ProductID,CategoryID,Product, Category, Selection")] ProductCategory productCategory)
        {

            var syncProduct = _context.Products
                .Where(x => x.Name == productCategory.Product.Name)
                .FirstOrDefault();
            var syncCategory = _context.Categories
                .Where(x => x.CategoryName == productCategory.Category.CategoryName)
                .FirstOrDefault();

            ProductCategory pc = new ProductCategory
            {
                CategoryID = syncCategory.CategoryID,
                ProductID = syncProduct.ID,
                Selection = productCategory.Selection
            };

            _context.Add(pc);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Products");

            //if(ModelState.IsValid)
            //{

            //    _context.Add(productCategory);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction("Index", "Products");
            //}

            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", productCategory.CategoryID);
            //ViewData["ProductID"] = new SelectList(_context.Products, "ID", "ID", productCategory.ProductID);
            //return View(productCategory);
        }

        // GET: ProductCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", productCategory.CategoryID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "ID", productCategory.ProductID);
            return View(productCategory);
        }

        // POST: ProductCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductCategoryID,ProductID,CategoryID,Selection")] ProductCategory productCategory)
        {
            if (id != productCategory.ProductCategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.ProductCategoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", productCategory.CategoryID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "ID", productCategory.ProductID);
            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories
                .Include(p => p.Category)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductCategoryID == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productCategory = await _context.ProductCategories.FindAsync(id);
            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategories.Any(e => e.ProductCategoryID == id);
        }
    }
}
