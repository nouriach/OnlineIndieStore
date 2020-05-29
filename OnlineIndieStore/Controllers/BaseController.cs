using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineIndieStore.Helpers;
using OnlineIndieStore.Models;

namespace OnlineIndieStore.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            List<Item> cart = new List<Item>();
            cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                int quantity = 0;
                foreach (var prod in cart)
                {
                    quantity += prod.Quantity;
                }
                ViewBag.cart = cart;
                ViewBag.quantity = quantity;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}