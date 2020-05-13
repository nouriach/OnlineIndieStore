using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineIndieStore.Models
{
    public class Image
    {
        // Primary Key

        // Properties
        [Column(TypeName ="nvarchar(50)")]
        public string Ttile { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image Name")]
        public string ImageName { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile ImageFile { get; set; }

        // Foreign Key
        [Key]
        public int ProductID { get; set; }

        // Navigational Properties
        public Product Product { get; set; }
    }
}
