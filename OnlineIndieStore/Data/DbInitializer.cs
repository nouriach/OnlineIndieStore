using Microsoft.AspNetCore.Identity;
using OnlineIndieStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.Data
{
    public class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Products.Any())
            {
                return;
            }
        }

        public static async Task InitializeUsers (UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager )
        {
            await roleManager.CreateAsync(new IdentityRole("Super_User"));

            string username = "superUser@onlineindiestore.com";
            var superUser = new AppUser
            {
                FirstName = "Super",
                UserName = username,
                Email = username
            };

            // create user and add to the database
            await userManager.CreateAsync(superUser, "P@SSWORDON3");

            // now get user and add them to a new role
            superUser = await userManager.FindByNameAsync(username);

            await userManager.AddToRoleAsync(superUser, "Super_User");
        }
    }
}
