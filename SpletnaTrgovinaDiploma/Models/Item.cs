using SpletnaTrgovinaDiploma.Data.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SpletnaTrgovinaDiploma.Helpers;

namespace SpletnaTrgovinaDiploma.Models
{
    public class Item : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        string[] ImageUrls => ImageUrl.Split(',');

        public string MainImageUrl => ImageUrls.GetIndexOfArrayOrEmpty(0);

        public string ProductCode { get; set; }
        public int? Availability { get; set; }

        //Relationships
        public List<BrandItem> BrandsItems { get; set; }
        public List<ItemDescription> Descriptions { get; set; }

        public NewItemViewModel CreateItemViewModel()
        {
            var imageUrls = ImageUrl.Split(',');

            return new NewItemViewModel
            {
                Id = Id,
                Name = Name,
                Description = Description,
                ShortDescription = ShortDescription,
                Price = Price,
                ImageUrl = ImageUrl,
                ImageUrl1 = imageUrls.GetIndexOfArrayOrEmpty(0),
                ImageUrl2 = imageUrls.GetIndexOfArrayOrEmpty(1),
                ImageUrl3 = imageUrls.GetIndexOfArrayOrEmpty(2),
                ImageUrl4 = imageUrls.GetIndexOfArrayOrEmpty(3),
                ImageUrl5 = imageUrls.GetIndexOfArrayOrEmpty(4),
                BrandIds = BrandsItems.Select(n => n.BrandId).ToList(),
                ProductCode = ProductCode,
                Availability = Availability
            };
        }
    }
}
