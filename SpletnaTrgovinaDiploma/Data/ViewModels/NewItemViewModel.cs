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

        public string ImageUrl { get; set; }

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

        public string ImageUrl1 { get; set; }
        public string ImageUrl2 { get; set; }
        public string ImageUrl3 { get; set; }
        public string ImageUrl4 { get; set; }
        public string ImageUrl5 { get; set; }

        [Display(Name = "Item image")]
        public IFormFile ImageFile1 { get; set; }
        [Display(Name = "Item image")]
        public IFormFile ImageFile2 { get; set; }
        [Display(Name = "Item image")]
        public IFormFile ImageFile3 { get; set; }
        [Display(Name = "Item image")]
        public IFormFile ImageFile4 { get; set; }
        [Display(Name = "Item image")]
        public IFormFile ImageFile5 { get; set; }

        public void UploadImagesAndSetImageUrls(IHostEnvironment hostEnvironment)
        {
            var commaSeparatedImageUrls = UploadImage(ImageFile1, ImageUrl1, hostEnvironment) + ",";
            commaSeparatedImageUrls += UploadImage(ImageFile2, ImageUrl2, hostEnvironment) + ",";
            commaSeparatedImageUrls += UploadImage(ImageFile3, ImageUrl3, hostEnvironment) + ",";
            commaSeparatedImageUrls += UploadImage(ImageFile4, ImageUrl4, hostEnvironment) + ",";
            commaSeparatedImageUrls += UploadImage(ImageFile5, ImageUrl5, hostEnvironment);

            ImageUrl = commaSeparatedImageUrls;
        }

        static string UploadImage(IFormFile imageFile, string oldImageUrl, IHostEnvironment hostEnvironment)
        {
            if (imageFile == null)
                return oldImageUrl;

            return imageFile.UploadImageFile(hostEnvironment);
        }
    }
}
