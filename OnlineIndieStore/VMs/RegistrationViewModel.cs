using OnlineIndieStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.VMs
{
    public class RegistrationViewModel
    {
        public AppUser AppUser { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password not match.")]
        public string ConfirmPassword { get; set; }
    }
}
