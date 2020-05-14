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
using OnlineIndieStore.Utilities;


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
        public IActionResult Index(string? order, string? categoryOrder, string? selectionOrder)
        {
            var appDbContext = _context.Products
                .Include(pc => pc.ProductCategories)
                    .ThenInclude(c => c.Category)
                .AsNoTracking();

            ViewBag.CatOptions = UtilityMethods.GetCategoryEnumsAsList();
            ViewBag.SelOptions = UtilityMethods.GetSelectionEnumsAsList();
            var displayAllProducts = UtilityMethods.GetAllLiveProducts(_context);
            var currentSelections = UtilityMethods.GetAllSelectionsInUse(_context);

            if (categoryOrder != null)
            {
                var returnFilteredCategories = FilterProductsByCategory(categoryOrder);
                return View(returnFilteredCategories);
            }

            if (order != null)
            {
                var returnOrderedProducts = OrderProducts(order, displayAllProducts);
                return View(returnOrderedProducts);
            }

            if (currentSelections.Contains(selectionOrder)) 
            { 
                var returnFilteredSelection = FilterProductsBySelection(selectionOrder);
                return View(returnFilteredSelection);
            }

            return View(displayAllProducts.OrderBy(x => x.Product.Name).ToList());
        }

        private List<DisplayProductViewModel> FilterProductsBySelection(string selectionOrder)
        {
            var appDbContext = _context.ProductCategories
                 .Include(p => p.Product)
                 .Include(c => c.Category)
                .AsNoTracking();

            List<DisplayProductViewModel> getAllMatchingProducts = new List<DisplayProductViewModel>();
            List<DisplayProductViewModel> getUniqueMatchingProducts = new List<DisplayProductViewModel>();

            try
            {
                foreach (var product in appDbContext)
                {
                    if (product.Selection.ToString() == selectionOrder)
                    {
                        DisplayProductViewModel dp = new DisplayProductViewModel();
                        dp.Product = product.Product;
                        dp.Categories = appDbContext
                            .Where(x => x.ProductID == product.Product.ID)
                            .Select(x => x.Category)
                            .ToList();
                        dp.Image = appDbContext
                            .Where(x => x.Product.Image.ProductID == product.Product.ID)
                            .Select(y => y.Product.Image)
                            .FirstOrDefault();

                        dp.Selection = selectionOrder;
                        getAllMatchingProducts.Add(dp);
                    }
                }

                var distinctProducts = getAllMatchingProducts.GroupBy(x => x.Product.ID).Select(y => y.First());

                foreach (var product in distinctProducts)
                {
                    getUniqueMatchingProducts.Add(product);
                }

                return getUniqueMatchingProducts.OrderBy(x => x.Product.Name).ToList();
            }

            catch
            {
                throw new NotImplementedException();
            }
        }

        private static List<DisplayProductViewModel> OrderProducts(string order, List<DisplayProductViewModel> displayProds)
        {
            try
            {
                switch (order)
                {
                    case "ByPriceAscending":
                        return 
                            displayProds.OrderBy(x => x.Product.Price).ToList();
                    case "ByPriceDescending":
                        return 
                            displayProds
                            .OrderByDescending(x => x.Product.Price)
                            .ToList();
                    case "ByNameDescending":
                        return 
                             displayProds
                            .OrderByDescending(x => x.Product.Name)
                            .ToList();
                    default:
                        return displayProds.OrderBy(x => x.Product.Name).ToList();
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public List<DisplayProductViewModel> FilterProductsByCategory(string categoryOrder)
        {
            var appDbContext = _context.ProductCategories
                 .Include(p => p.Product)
                 .Include(c => c.Category)
                .AsNoTracking();

            try
            {
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
                    dp.Selection = i.Selection.ToString();
                    dp.Image = appDbContext
                        .Where(x => x.Product.Image.ProductID == i.ProductID)
                        .Select(x => x.Product.Image).FirstOrDefault();

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
                var allCategoriesInTable = from categ in _context.Categories
                                           select categ;
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
        public async Task<IActionResult> Details(string? productName)
        {
            if (productName == null)
            {
                return NotFound();
            }


            var findMatchingProduct = await _context.ProductCategories
                .Include(p => p.Category)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Product.Name == productName);

            DisplayProductViewModel productDetails = new DisplayProductViewModel
            {
                Product = findMatchingProduct.Product,
                Categories = _context.ProductCategories.Where(x => x.ProductID == findMatchingProduct.ProductID).Select(y => y.Category).ToList(),
                Selection = findMatchingProduct.Selection.ToString(),
                Image = _context.ProductCategories
                .Where(i => i.Product.Image.ProductID == findMatchingProduct.ProductID)
                .Select(img => img.Product.Image)
                .FirstOrDefault()
            };


            if (productDetails == null)
            {
                return NotFound();
            }

            return View(productDetails);
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
