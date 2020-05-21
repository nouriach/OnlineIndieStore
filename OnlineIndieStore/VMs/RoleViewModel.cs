using Microsoft.AspNetCore.Identity;
using OnlineIndieStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.VMs
{
    public class RoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
        public IQueryable<IdentityRole> IdentityRole { get; set; }
        public List<AppUser> AppUser { get; set; }

    }
}
