using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineIndieStore.Models;
using OnlineIndieStore.VMs;

namespace OnlineIndieStore.Controllers
{
    public class AccountController : Controller
    {

        private ILogger _logger;
        private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> _user;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(ILogger<AccountController> logger, Microsoft.AspNetCore.Identity.UserManager<AppUser> user, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _user = user;
            _signInManager = signInManager;
        }
       
        public IActionResult Index()
        {
            
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser()
                {
                    FirstName = model.AppUser.FirstName,
                    UserName = model.AppUser.UserName,
                    Email = model.AppUser.Email
                };
                var result = await _user.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, true);
                }

                foreach (var errors in result.Errors)
                {
                    ModelState.AddModelError("", errors.Description);
                }
            }

            return View();
        }
    }
}