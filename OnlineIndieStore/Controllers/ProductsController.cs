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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;

namespace OnlineIndieStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
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

            return View(await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DisplayProductViewModel dPvm = new DisplayProductViewModel();

            var product = await _context.Products
                .Include(pc => pc.ProductCategories)
                    .ThenInclude(c => c.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            dPvm.Product = product;
            dPvm.Categories = product.ProductCategories
                .Where(x => x.ProductID == product.ID)
                .Select(c => c.Category)
                .ToList();
            dPvm.Selection = product.ProductCategories
                .Where(x => x.ProductID == product.ID)
                .Select(c => c.Selection.ToString())
                .FirstOrDefault();
            dPvm.Image = product.ProductCategories
                .Where(i => i.Product.Image.ProductID == product.ID)
                .Select(img => img.Product.Image)
                .FirstOrDefault();
                

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "GeneralUser, Admin, SuperAdmin")]

        public IActionResult Create()
        {
            ViewBag.Options = UtilityMethods.GetCategoryEnumsAsList();

            return View();
        }

        // POST: Products/Create
        [Authorize(Roles = "GeneralUser, Admin, SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategoryViewModel newProdCat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<ProductCategory> pc = new List<ProductCategory>();
                    Models.Image newImg = new Models.Image();
                    Product newProduct = new Product();

                    if (newProdCat.Product != null)
                    {
                        newProduct = newProdCat.Product;
                        _context.Add(newProduct);
                        _context.SaveChanges();

                    }
                    var syncProduct = _context.Products
                        .Where(x => x.Name == newProdCat.Product.Name)
                        .FirstOrDefault();

                    if (newProdCat.Image != null)
                    {
                        // Get and save Image to wwwroot/image
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(newProdCat.Image.ImageFile.FileName);
                        string extension = Path.GetExtension(newProdCat.Image.ImageFile.FileName);
                        //newProdCat.Image.ImageName = fileName+DateTime.Now.ToString("yymmssfff") + extension;
                        newProdCat.Image.ImageName = fileName + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/", fileName + extension);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await newProdCat.Image.ImageFile.CopyToAsync(fileStream);
                        }

                        newImg = newProdCat.Image;
                        newImg.ProductID = syncProduct.ID;

                        _context.Add(newImg);
                        _context.SaveChanges();
                    }
                    var syncImage = _context.Images
                        .Where(x => x.ImageName == newProdCat.Image.ImageName)
                        .FirstOrDefault();

                    if (newProdCat.Category.Count > 0)
                    {
                        foreach(var test in newProdCat.Category)
                        {
                            var syncCategory = _context.Categories
                                .Where(x => x.CategoryName == test)
                                .FirstOrDefault();

                            ProductCategory newTest = new ProductCategory();

                            newTest.CategoryID = syncCategory.CategoryID;
                            newTest.ProductID = syncProduct.ID;
                            // newTest.Product.Image = syncImage;
                            newTest.Selection = newProdCat.ProductCategory.Selection;

                            pc.Add(newTest);
                        }
                    }
                    
                    foreach (var newProdCatInstance in pc)
                    {
                        // no presence of Image in here
                        _context.Add(newProdCatInstance);
                        await _context.SaveChangesAsync();
                    }
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
