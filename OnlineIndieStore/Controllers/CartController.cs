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
            List<Item> cart = new List<Item>();
            cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            // when the cart is captured it doesn't bring in the Product's Image reference.
            if (cart != null)
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => item.Product.Price * item.Quantity);
            }
            return View();
        }

        public IActionResult Buy (string id)
        {
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                Item item = new Item()
                {
                    Product = _context.Products.Where(p => p.ID.ToString() == id).FirstOrDefault(),
                    Quantity = 1
                };
                cart.Add(item);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    Item newItem = new Item()
                    {
                        Product = _context.Products.Where(p => p.ID.ToString() == id).FirstOrDefault(),
                        Quantity = 1
                    };
                    cart.Add(newItem);
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

        public IActionResult ChangeQuantity(string id, string quantityChange)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);

            if(index != -1)
            {
                if ((quantityChange == "minus") && (cart[index].Quantity != 1))
                {
                    cart[index].Quantity -= 1;
                }
                else if (quantityChange == "plus")
                {
                    cart[index].Quantity += 1;
                }
                else
                {
                    return RedirectToAction("Index");
                }
            } 
            else
            {
                return NotFound();
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }
        // UTILITY METHODS

        private int isExist(string id)
        {
            int idToInt = int.Parse(id);
            List<Item> cart = new List<Item>();
           
            cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.ID.Equals(idToInt))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
