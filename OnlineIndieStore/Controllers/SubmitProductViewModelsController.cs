using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineIndieStore.Data;
using OnlineIndieStore.Models;
using OnlineIndieStore.ViewModels;

namespace OnlineIndieStore.Controllers
{
    public class SubmitProductViewModelsController : Controller
    {
        private readonly AppDbContext _context;

        public SubmitProductViewModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SubmitProductViewModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.SubmitProductViewModel.ToListAsync());
        }

        // GET: SubmitProductViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submitProductViewModel = await _context.SubmitProductViewModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (submitProductViewModel == null)
            {
                return NotFound();
            }

            return View(submitProductViewModel);
        }

        // GET: SubmitProductViewModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SubmitProductViewModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Price,Description,ImageUrl")] SubmitProductViewModel submitProductViewModel)
        {
            Product product = new Product
            {
                ID = submitProductViewModel.ID,
                Name = submitProductViewModel.Name,
                Price = submitProductViewModel.Price,
                Description = submitProductViewModel.Description,
                ImageUrl = submitProductViewModel.ImageUrl
            };

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes");
            }
            return View(product);
        }

        // GET: SubmitProductViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submitProductViewModel = await _context.SubmitProductViewModel.FindAsync(id);
            if (submitProductViewModel == null)
            {
                return NotFound();
            }
            return View(submitProductViewModel);
        }

        // POST: SubmitProductViewModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Price,Description,ImageUrl")] SubmitProductViewModel submitProductViewModel)
        {
            if (id != submitProductViewModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submitProductViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmitProductViewModelExists(submitProductViewModel.ID))
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
            return View(submitProductViewModel);
        }

        // GET: SubmitProductViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submitProductViewModel = await _context.SubmitProductViewModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (submitProductViewModel == null)
            {
                return NotFound();
            }

            return View(submitProductViewModel);
        }

        // POST: SubmitProductViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submitProductViewModel = await _context.SubmitProductViewModel.FindAsync(id);
            _context.SubmitProductViewModel.Remove(submitProductViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubmitProductViewModelExists(int id)
        {
            return _context.SubmitProductViewModel.Any(e => e.ID == id);
        }
    }
}
