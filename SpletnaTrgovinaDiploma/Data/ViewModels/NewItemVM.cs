using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Models
{
    public class NewItemVM
    {
        public int Id { get; set; }

        [Display(Name ="Item name")]
        [Required(ErrorMessage="Name is required")]
        public string Name { get; set; }

        [Display(Name = "Item description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Price in €")]
        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        [Display(Name = "Item image URL")]
        [Required(ErrorMessage = "Item image URL is required")]
        public string ImageURL { get; set; }

        [Display(Name = "Select a category")]
        [Required(ErrorMessage = "Item category is required")]
        public ItemCategory ItemCategory { get; set; }

        //Relationships
        [Display(Name = "Select brand(s)")]
        [Required(ErrorMessage = "Item brand(s) is required")]
        public List<int> BrandIds { get; set; }
    }
}
