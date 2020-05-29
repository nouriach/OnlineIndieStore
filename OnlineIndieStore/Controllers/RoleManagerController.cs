using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineIndieStore.Models;
using OnlineIndieStore.VMs;

namespace OnlineIndieStore.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]

    public class RoleManagerController : BaseController
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

        [Authorize(Roles = "SuperAdmin")]
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
                    return RedirectToAction("Index", "RoleManager");
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
        [Authorize(Roles = "SuperAdmin")]
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

        [Authorize(Roles = "SuperAdmin")]
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
        [Authorize(Roles = "SuperAdmin")]

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
        [Authorize(Roles = "SuperAdmin")]

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
            List<EditsUsersInRolesViewModel> userRoleList = new List<EditsUsersInRolesViewModel>();

            var matchingUser = await _user.FindByIdAsync(id);
            ViewBag.FirstName = matchingUser.FirstName;
            ViewBag.UserName = matchingUser.UserName ;
            ViewBag.Email = matchingUser.Email ;

            if (matchingUser == null)
            {
                ViewBag.ErrorMessages = $"Role of given id {id} is not found.";
                return View("NotFound");
            }

            foreach(var r in this.rolesManager.Roles)
            {
                EditsUsersInRolesViewModel userRole = new EditsUsersInRolesViewModel();
                // IdentityRole newRole = r;
                userRole.IdentityRoles = r;
                userRole.AppUser = matchingUser;
                if (await _user.IsInRoleAsync(matchingUser, r.Name))
                {
                    userRole.IsChecked = true;
                }
                else
                {
                    userRole.IsChecked = false;
                }
                userRoleList.Add(userRole);
            }
            return View(userRoleList);
        }
        [Authorize(Roles = "SuperAdmin")]

        [HttpPost]
        public async Task<IActionResult> EditUsersInRoles(List<EditsUsersInRolesViewModel> model, string id)
        {
            // find user that is being updated

            var user = await _user.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessages = $"Role of given id {id} is not found.";
                return View("NotFound");
            }
            else
            {
                IdentityResult result = null;
                foreach (var checkbox in model)
                {
                    if ((checkbox.IsChecked == true) && !(await _user.IsInRoleAsync(user, checkbox.IdentityRoles.Name)))
                    {
                        // tie user role to use and save
                        result = await _user.AddToRoleAsync(user, checkbox.IdentityRoles.Name);
                    }
                    else if (!(checkbox.IsChecked == true) && (await _user.IsInRoleAsync(user, checkbox.IdentityRoles.Name)))
                    {
                        result = await _user.RemoveFromRoleAsync(user, checkbox.IdentityRoles.Name);
                    }
                    else
                    {
                        continue;
                    }
                }
                if (result.Succeeded)
                {
                    return RedirectToAction("EditUsersInRoles", "RoleManager" );
                }
            }
            return View("NotFound");
        }
    }
}