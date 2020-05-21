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
        public List<IdentityRole> IdentityRoles { get; set; }
    }
}
