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
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            var appDbContext =_context.Products;

            // Search Box
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParam"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var products = from p in _context.Products
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 3;

            // Filter products
            //switch (order)
            //{
            //    case "ByPriceAscending":
            //        return View(
            //            await appDbContext
            //            .OrderBy(x => x.Price)
            //            .ToListAsync()
            //            );
            //    case "ByPriceDescending":
            //        return View(
            //            await appDbContext
            //            .OrderByDescending(x => x.Price)
            //            .ToListAsync()
            //            );
            //    case "ByNameDescending":
            //        return View(
            //            await appDbContext
            //            .OrderByDescending(x => x.Name)
            //            .ToListAsync()
            //            );
            //    default:
            //        return View(await appDbContext.OrderBy(x => x.Name).ToListAsync());
            //}

            return View(await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(pc => pc.ProductCategories)
                    .ThenInclude(c => c.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            //List<Category> categorySelectList = new List<Category>();
            //foreach (var i in _context.Categories)
            //{
            //    categorySelectList.Add(i);
            //}

            //Product newProduct = new Product();
            //ProductCategory newProdCat = new ProductCategory();

            //ProductCategoryViewModel produCatViewModel = new ProductCategoryViewModel()
            //{
            //    //Category = categorySelectList,
            //    Product = newProduct,
            //    ProductCategory = newProdCat
            //};

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategoryViewModel newProdCat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (newProdCat.Product != null)
                    {
                        Product newProduct = newProdCat.Product;
                        _context.Add(newProduct);
                        _context.SaveChanges();
                    }
                    var syncProduct = _context.Products
                        .Where(x => x.Name == newProdCat.Product.Name)
                        .FirstOrDefault();
                    var syncCategory = _context.Categories
                        .Where(x => x.CategoryName == newProdCat.Category.CategoryName)
                        .FirstOrDefault();

                    ProductCategory pc = new ProductCategory
                    {
                        CategoryID = syncCategory.CategoryID,
                        ProductID = syncProduct.ID,
                        Selection = newProdCat.ProductCategory.Selection
                    };

                    _context.Add(pc);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "ProductCategories");
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes");
            }
            return View(newProdCat);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
          if (id == null)
            {
                return NotFound();
            }
            var productToUpdate = await _context.Products.FirstOrDefaultAsync(p => p.ID == id);
            if (await TryUpdateModelAsync<Product>(
                productToUpdate,
                "",
               p => p.Name, p => p.Price, p => p.Description, p => p.ImageUrl ))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes.");
                }
            }
            return View(productToUpdate);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed.Try again.";
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }
    }
}
