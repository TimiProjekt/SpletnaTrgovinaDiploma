using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Data.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public ItemCategory ItemCategory { get; set; }

        //Relationships
        public List<BrandItem> BrandsItems { get; set; }
    }
}
