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
            List<EditsUsersInRolesViewModel> userRoleList = new List<EditsUsersInRolesViewModel>();

            var matchingUser = await _user.FindByIdAsync(id);
            ViewBag.User = matchingUser.FirstName.ToString();


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

            // In the blog it suggests the View needs to have a List<ViewModel>
            // 1. The model in here that is instantiated needs to be a List<EditsUsersInRolesViewModel>
            // 2. Then for every role in the system, do a for loop which
            /***
            a.Creates a new instance of EditsUsersInRolesViewModel()
            b.Adds the matching role to the viewmodel
            c.Then checks whether the role is already assigned to the User defined at the top (matchingUser)
            d.If it is assigned, then mark the 'IsSelected' bool as 'true' in the viewmodel and vice versa
            e.then after each instance of the model has been added with a true or a false, add it to the List<EditsUsersInRolesViewModel>
            ***/

            // link: https://sagarjaybhay.com/rolemanager-in-asp-net-core-2019/

            // So what does the view model look like?
            /*****
                public string Id { get; set; }
                public AppUser AppUser { get; set; } (maybe not...? It looks like the UserID should be stored as an ID in the ViewBag)
                public IdentityRole IdentityRole { get; set; }
                public bool IsChecked { get; set; }
             *****/

            return View(userRoleList);
        }

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


/************

    HTTP GET:

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

    HTTP POST

    else
            {
                // List all roles
                List<IdentityRole> availableIdentityRoles = new List<IdentityRole>();
                // initiate a new list of IdentityRoles
                List<IdentityRole> ir = new List<IdentityRole>();
                // Create new Role Model to pass to View, add the relevant user
                EditsUsersInRolesViewModel userRoleModel = new EditsUsersInRolesViewModel()
                {
                    Id = (user.Id),
                    AppUser = user,
                    IsChecked = false
                };
                

                // scan through the List<string> admin roles elected and store in a new List
                var checkedRoles = IsChecked.Where(x => x != "false").Select(x => x);

                // loop through the stored Roles in the database
                foreach (var r in this.rolesManager.Roles)
                {
                    availableIdentityRoles.Add(r);
                    
                    // if the stored roles in the database are also included in the checked roles add to the new View Model instance
                    if (checkedRoles.Contains(r.ToString()))
                    {
                        
                        ir.Add(r);
                    }
                }
                userRoleModel.IdentityRoles = ir;

            ViewBag.AvailableRoles = availableIdentityRoles;
 */
