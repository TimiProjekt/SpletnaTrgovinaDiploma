﻿using SpletnaTrgovinaDiploma.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Models
{
    public class Brand:IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Profile Picture")]
        [Required(ErrorMessage = "Profile Picture is required")]
        public string ProfilePictureURL  { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 50")]
        public string Name { get; set; }
        

        //Relationships
        public List<Brand_Item> Brands_Items { get; set; }
    }
}