using SpletnaTrgovinaDiploma.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpletnaTrgovinaDiploma.Models
{
    public class NewItemViewModel
    {
        public NewItemViewModel()
        {
            BrandIds = new List<int>();
            ItemDescriptions = new List<ItemDescription>();
        }

        public int Id { get; set; }

        [Display(Name = "Item name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Display(Name = "Item description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Short item description")]
        [Required(ErrorMessage = "Description is required")]
        public string ShortDescription { get; set; }

        [Display(Name = "Price in €")]
        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        [Display(Name = "Item image URL")]
        [Required(ErrorMessage = "Item image URL is required")]
        public string ImageUrl { get; set; }

        [Display(Name = "Select a category")]
        [Required(ErrorMessage = "Item category is required")]
        public ItemCategory ItemCategory { get; set; }

        [Display(Name = "Product code")]
        public string ProductCode { get; set; }

        [Display(Name = "Availability")]
        public int? Availability { get; set; }

        //Relationships
        [Display(Name = "Select brand(s)")]
        [Required(ErrorMessage = "Item brand(s) is required")]
        public List<int> BrandIds { get; set; }

        // [Display(Name = "Select brand(s)")]
        // [Required(ErrorMessage = "Item brand(s) is required")]
        public List<ItemDescription> ItemDescriptions { get; set; }
    }
}
