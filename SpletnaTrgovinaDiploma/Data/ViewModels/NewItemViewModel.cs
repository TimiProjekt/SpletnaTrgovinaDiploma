using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace SpletnaTrgovinaDiploma.Models
{
    public class NewItemViewModel
    {
        public NewItemViewModel()
        {
            BrandIds = new List<int>();
            BrandNames = new List<string>();
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
        public decimal? Price { get; set; }

        [Display(Name = "Item image URL")]
        public string ImageUrl { get; set; }

        [Display(Name = "Item image")]
        [Required(ErrorMessage = "Item image is required")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Product code")]
        public string ProductCode { get; set; }

        [Display(Name = "Availability")]
        public int? Availability { get; set; }

        //Relationships
        [Display(Name = "Select brand(s)")]
        [Required(ErrorMessage = "Item brand(s) is required")]
        public List<int> BrandIds { get; set; }       
        
        //Relationships
        public List<string> BrandNames { get; set; }

        // [Display(Name = "Select brand(s)")]
        // [Required(ErrorMessage = "Item brand(s) is required")]
        public List<ItemDescription> ItemDescriptions { get; set; }

        public void UploadImageAndSetImageUrl(IHostEnvironment hostEnvironment)
        {
            if (ImageFile == null)
                return;

            var imageUrl = ImageFile.UploadImageFile(hostEnvironment);
            if (!string.IsNullOrEmpty(imageUrl))
                ImageUrl = imageUrl;
        }
    }
}
