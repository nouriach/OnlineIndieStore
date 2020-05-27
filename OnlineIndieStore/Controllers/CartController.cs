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
    public class CartController : Controller
    {
        private AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Product.Price * item.Quantity);
            return View();
        }

        public IActionResult Buy (string id)
        {
            Product product = new Product();
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item
                    {
                        Product = _context.Products.Where(p => p.ID.ToString() == id).FirstOrDefault(),
                        Quantity = 1
                    });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Remove(string id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        private int isExist(string id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                // Should this ToString() the Product ID?
                if (cart[i].Product.ID.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}