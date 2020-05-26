using Microsoft.AspNetCore.Identity;
using OnlineIndieStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.VMs
{
    public class EditsUsersInRolesViewModel
    {
        public string Id { get; set; }
        public AppUser AppUser { get; set; }
        public IdentityRole IdentityRoles { get; set; }
        public bool IsChecked { get; set; }

    }
}
