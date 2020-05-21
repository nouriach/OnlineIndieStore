using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineIndieStore.Models;
using OnlineIndieStore.VMs;

namespace OnlineIndieStore.Controllers
{
    public class RoleManagerController : Controller
    {
        public RoleManager<IdentityRole> _rolesManager { get; set; }
        private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> _user;

        public RoleManagerController(RoleManager<IdentityRole> rolesManager, UserManager<AppUser> userManager)
        {
            _user = userManager;
            _rolesManager = rolesManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        /***** CREATE NEW ROLES *****/

        [HttpGet]
        public IActionResult CreateRoles()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoles(RoleViewModel roleView)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = roleView.RoleName
                };
                IdentityResult result = await _rolesManager.CreateAsync(role);
                    
                if (result.Succeeded)
                {
                    return RedirectToAction("ListOfRoles", "Rolemanag");
                }
                foreach (var identityErrorLE in result.Errors)
                {
                    ModelState.AddModelError("", identityErrorLE.Description);
                }
            }
            return View(roleView);
        }
    }
}
 