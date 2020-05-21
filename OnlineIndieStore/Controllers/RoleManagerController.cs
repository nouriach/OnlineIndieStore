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
        public RoleManager<IdentityRole> rolesManager { get; set; }
        private readonly UserManager<AppUser> _user;

        public RoleManagerController(RoleManager<IdentityRole> rolesManager, UserManager<AppUser> userManager)
        {
            _user = userManager;
            this.rolesManager = rolesManager;
        }
        public IActionResult Index(RoleViewModel rvm)
        {
            var list = this.rolesManager.Roles;
            List<AppUser> userList = new List<AppUser>();

            foreach (var users in _user.Users)
            {
                userList.Add(users);
            }

            RoleViewModel listRolesViewModel = new RoleViewModel()
            {
                IdentityRole = list,
                AppUser = userList.OrderBy(x => x.FirstName).ToList()
            };

            return View(listRolesViewModel);
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
                IdentityResult result = await rolesManager.CreateAsync(role);
                    
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

        /***** LIST ROLES *****/
        public IActionResult ListOfRoles()
        {
            var list = this.rolesManager.Roles;
            return View(list);
        }

        /***** EDIT ROLES *****/

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var matchingRole = await this.rolesManager.FindByIdAsync(id);
            if (matchingRole == null)
            {
                ViewBag.ErrorMessages = $"Role of given id {id} is not found.";
                return View("NotFound");
            }
            else
            {
                var model = new EditRoleViewModel()
                {
                    RoleName = matchingRole.Name,
                    Id = (matchingRole.Id),

                };
                foreach (var users in _user.Users)
                {
                    if (await _user.IsInRoleAsync(users, matchingRole.Name))
                    {
                        model.Users.Add(users.UserName);
                    }

                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel newModel)
        {
            var matchingRole = await this.rolesManager.FindByIdAsync(newModel.Id);
            if (matchingRole == null)
            {
                ViewBag.ErrorMessages = $"Role of given id {newModel.Id} is not found.";
                return View("NotFound");
            }

            else
            {
                matchingRole.Name = newModel.RoleName;
                var res = await this.rolesManager.UpdateAsync(matchingRole);
                if (res.Succeeded)
                {
                    return RedirectToAction("Index", "RoleManager");
                }
                foreach (var erros in res.Errors)
                {
                    ModelState.AddModelError("", erros.Description);
                }
            }
              
            return View(newModel);
        }

        /***** DELETE ROLES *****/

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var matchingRole = await this.rolesManager.FindByIdAsync(id);
            if (matchingRole == null)
            {
                ViewBag.ErrorMessages = $"Role of given id {id} is not found.";
                return View("NotFound");
            }
            else
            {
                var model = new EditRoleViewModel()
                {
                    RoleName = matchingRole.Name,
                    Id = (matchingRole.Id),

                };
                foreach (var users in _user.Users)
                {
                    if (await _user.IsInRoleAsync(users, matchingRole.Name))
                    {
                        model.Users.Add(users.UserName);
                    }

                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(EditRoleViewModel newModel)
        {
            var matchingRole = await this.rolesManager.FindByIdAsync(newModel.Id);
            if (matchingRole == null)
            {
                ViewBag.ErrorMessages = $"Role of given id {newModel.Id} is not found.";
                return View("NotFound");
            }

            else
            {
                matchingRole.Name = newModel.RoleName;
                var res = await this.rolesManager.DeleteAsync(matchingRole);
                if (res.Succeeded)
                {
                    return RedirectToAction("Index", "RoleManager");
                }
                foreach (var erros in res.Errors)
                {
                    ModelState.AddModelError("", erros.Description);
                }
            }
            return View(newModel);
        }

        /****** Assign Role to a User *******/

        [HttpGet]
        public async Task<IActionResult> EditUsersInRoles(string id)
        {
            var matchingUser = await _user.FindByIdAsync(id);
            if (matchingUser == null)
            {
                ViewBag.ErrorMessages = $"Role of given id {id} is not found.";
                return View("NotFound");
            }
            else
            {
                List<IdentityRole> userIdentityRoles = new List<IdentityRole>();
                List<IdentityRole> availableIdentityRoles = new List<IdentityRole>();

                foreach (var role in this.rolesManager.Roles)
                {
                    availableIdentityRoles.Add(role);

                    if (await _user.IsInRoleAsync(matchingUser, role.ToString()))
                    {
                        userIdentityRoles.Add(role);
                    }
                }
                EditsUsersInRolesViewModel model = new EditsUsersInRolesViewModel()
                {
                    Id = (matchingUser.Id),
                    AppUser = matchingUser,
                    IdentityRoles = userIdentityRoles
                };

                ViewBag.AvailableRoles = availableIdentityRoles;

                return View(model);
            }

            // Create a new view model for displaying it all
            // add the user to the view model
            // loop through user to see if any roles have historically been assigned

            return View();
        }
    }
}
 