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
        public async Task<IActionResult> Index(string? order, string? selectionOrder, string? categoryOrder)
        {

            var appDbContext = _context.Products
                .Include(pc => pc.ProductCategories)
                    .ThenInclude(c => c.Category)
                .AsNoTracking();

            var productCategory = _context.ProductCategories
                .Include(p => p.Product)
                .Include(p => p.Category)
                .Where(c => c.CategoryID == 5)
                .AsNoTracking();
                


            List<string> categories = Enum.GetNames(typeof(CategoryName)).OrderBy(x => x).ToList();
            ViewBag.CatOptions = categories;

            List<string> selections = Enum.GetNames(typeof(Selection)).OrderBy(y => y).ToList();
            ViewBag.SelOptions = selections;

            if (categoryOrder != null)
            {
                // store all category IDs that match the passed categoryOrder
                int categoryId = 0;

                // get all ProductCategories
                var getRelevantProducts = _context.ProductCategories;

                // get all Categories
                var productsByCategory = _context.Categories;

                // store all final Products
                List<Product> finalProducts = new List<Product>();

                // Go through every available 'Category' and store the ID
                foreach (var findCategoryId in productsByCategory)
                {
                    if (findCategoryId.CategoryName.ToString() == categoryOrder)
                    {
                        categoryId = findCategoryId.CategoryID;
                    }
                }



                return View();
            }

            switch (order)
            {
                case "ByPriceAscending":
                    return View(
                        await productCategory
                        .OrderBy(x => x.Product.Price)
                        .ToListAsync()
                        );
                case "ByPriceDescending":
                    return View(
                        await productCategory
                        .OrderByDescending(x => x.Product.Price)
                        .ToListAsync()
                        );
                case "ByNameDescending":
                    return View(
                        await productCategory
                        .OrderByDescending(x => x.Product.Name)
                        .ToListAsync()
                        );
                default:
                    return View(await productCategory.OrderBy(x => x.Product.Name).ToListAsync());
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



//public async Task<StakeholderResponse> GetServiceAccessStakeholder(CreateServiceStakeholder request)
//{
//    var serviceStakeholder = await _context.ServiceAccess
//        .AsNoTracking()
//        .Where(x =>
//            x.StakeholderID == request.StakeholderID &&
//            x.ServiceID == request.ServiceID)
//        .Select(ObjectMapper.GetStakeholderResponseFromServiceAccess())
//        .FirstOrDefaultAsync();

//    if (serviceStakeholder == null)
//        throw new NotFoundException("Could not find service stakeholder");

//    return serviceStakeholder;
//}


    //// start building a query to this table
    //IQueryable<Service> query = _context.Service
    //            .AsNoTracking()
    //                .Where(x => x.Archived == false);                   //check service not archived
    //                                                                    // check if tag exists
    //if (!string.IsNullOrEmpty(tag))
    //{
    //    // filters services, checks if service has the tag that has been passed through as an argument
    //    query = query
    //        .Where(x => x.ServiceTags
    //            .Any(z => z.Tag.Name == tag));
    //}

    //IQueryable<ServicesItem> transformedQuery = query
    //    .OrderBy(x => x.Name) // orders by name
    //    .Select(ObjectMapper.GetServicesItemFromService());  // transforms Iqueryable from Service to ServicesItem
    //                                                         // send query to database and return result as a list
    //var executedQueryResult = await transformedQuery.ToListAsync();

