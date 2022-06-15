using SpletnaTrgovinaDiploma.Data.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpletnaTrgovinaDiploma.Models
{
    public class Brand : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Profile Picture")]
        [Required(ErrorMessage = "Profile Picture is required")]
        public string ProfilePictureUrl { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 50")]
        public string Name { get; set; }


        //Relationships
        public List<BrandItem> BrandsItems { get; set; }
    }
}