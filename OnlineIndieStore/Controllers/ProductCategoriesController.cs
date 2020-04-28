using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineIndieStore.Data;
using OnlineIndieStore.Models;

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
        public async Task<IActionResult> Index(string? order)
        {
            var appDbContext = _context.ProductCategories
                .Include(p => p.Category)
                .Include(p => p.Product);

            List<string> categories = Enum.GetNames(typeof(CategoryName)).OrderBy(x => x).ToList();
            ViewBag.CatOptions = categories;

            List<string> selections = Enum.GetNames(typeof(Selection)).OrderBy(y => y).ToList();
            ViewBag.SelOptions = selections;

            switch (order)
            {
            case "ByPriceAscending":
                return View(
                    await appDbContext
                    .OrderBy(x => x.Product.Price)
                    .ToListAsync()
                    );
            case "ByPriceDescending":
                return View(
                    await appDbContext
                    .OrderByDescending(x => x.Product.Price)
                    .ToListAsync()
                    );
            case "ByNameDescending":
                return View(
                    await appDbContext
                    .OrderByDescending(x => x.Product.Name)
                    .ToListAsync()
                    );
            default:
                return View(await appDbContext.OrderBy(x => x.Product.Name).ToListAsync());
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductCategoryID,ProductID,CategoryID,Product, Category, Selection")] ProductCategory productCategory)
        {

            var syncProduct = _context.Products.Where(x => x.Name == productCategory.Product.Name).FirstOrDefault();
            var syncCategory = _context.Categories.Where(x => x.CategoryName == productCategory.Category.CategoryName).FirstOrDefault();

            //foreach(var prodcat in _context.ProductCategories)
            //{
            //    if (prodcat.ProductID == syncProduct.ID)
            //    {
            //        prodcat.Category.CategoryName
            //    }
            //}
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
