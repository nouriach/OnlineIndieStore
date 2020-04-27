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
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create(string searchString)
        {

            List<string> categories = Enum.GetNames(typeof(CategoryName)).OrderBy(x => x).ToList();
            List<string> availableCategories = new List<string>();

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach(var option in categories)
                {
                    if (option.ToUpper().Contains(searchString.ToUpper())) 
                    {
                        availableCategories.Add(option);
                    }
                }
            }
            ViewBag.Results = availableCategories.OrderBy(x => x).ToList();
            ViewBag.Options = categories;

            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName", "IsChecked")] List<CategoryName> IsChecked)
        {
            /********
             
            1. The passed argument needs to be a List<Category>
            2. I think there needs to be a hidden 'id' field on the checkbox
            3. Why does 'IsChecked' worked but 'CategoryName' currently doesn't?
            4. Looks like each instance of the below doesn't bring a new ID with. No auto-increment.

            https://www.codeproject.com/articles/1078491/creating-forms-in-asp-net-mvc
            https://sensibledev.com/mvc-checkbox-and-checkboxlist/

            ************/
            foreach (var c in IsChecked)
            {
                var findCategoryId = _context.Categories.Count();
                Category newCat = new Category
                {
                    CategoryID = findCategoryId,
                    CategoryName = c,
                    IsChecked = true
                };
                _context.Categories.Add(newCat);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Categories");
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryID,CategoryName")] Category category)
        {
            if (id != category.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryID))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryID == id);
        }
    }
}
