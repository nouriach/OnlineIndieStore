using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineIndieStore.Data;
using OnlineIndieStore.Helpers;
using OnlineIndieStore.Models;

namespace OnlineIndieStore.Controllers
{
    public class FavouriteController : Controller
    {
        private AppDbContext _context;

        public FavouriteController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Item> favourites = new List<Item>();
            favourites = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "favourites");

            if (favourites != null)
            {
                foreach (var product in favourites)
                {
                    product.Product.Image = _context.Images.Where(x => x.ProductID == product.Product.ID).FirstOrDefault();
                }
                ViewBag.favourites = favourites;
            }
            return View();
        }

        public IActionResult AddProductToFavourite(string id)
        {
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "favourites") == null)
            {
                List<Item> favourites = new List<Item>();
                Item newItem = new Item()
                {
                    Product = _context.Products.Where(x => x.ID.ToString() == id).FirstOrDefault()
                };
                favourites.Add(newItem);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "favourites", favourites);
            }
            else
            {
                List<Item> favourites = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "favourites");
                if (favourites.Any(p => p.Product.ID.ToString() == id))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    Item newItem = new Item()
                    {
                        Product = _context.Products.Where(x => x.ID.ToString() == id).FirstOrDefault()
                    };
                    favourites.Add(newItem);
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "favourites", favourites);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveProductFromFavourite(string id)
        {
            List<Item> favourites = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "favourites");
            int index = isExist(id);
            favourites.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "favourites", favourites);
            return RedirectToAction("Index");
        }

        private int isExist(string id)
        {
            int idToInt = int.Parse(id);
            List<Item> favourites = new List<Item>();

            favourites = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "favourites");

            for (int i = 0; i < favourites.Count; i++)
            {
                if (favourites[i].Product.ID.Equals(idToInt))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}